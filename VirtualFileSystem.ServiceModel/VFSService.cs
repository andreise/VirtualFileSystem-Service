using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Web.Configuration;
using System.Xml;
using static System.FormattableString;

namespace VirtualFileSystem.ServiceModel
{
    using VirtualFileSystem.Common.Console;
    using VirtualFileSystem.Common.Security;
    using VirtualFileSystem.Model;
    using VirtualFileSystem.Model.Console;
    using Security;
    using Security.Cryptography;

    /// <summary>
    /// Virtual File System Service
    /// </summary>
    [ServiceBehavior(
        AutomaticSessionShutdown = false,
        ConcurrencyMode = ConcurrencyMode.Single,
        InstanceContextMode = InstanceContextMode.Single
    )]
    public class VFSService : IVFSService
    {
        private readonly IFileSystemConsole Console = FileSystemConsoleFactory.Create(FileSystemFactory.Default);

        private IVFSServiceCallback GetCallbackChannel() => OperationContext.Current.GetCallbackChannel<IVFSServiceCallback>();

        private TokenProvider TokenProvider => TokenProvider.Default;

        private readonly Dictionary<string, UserSessionInfo> connectedUsers = new Dictionary<string, UserSessionInfo>(UserNameComparerProvider.Default);

        private void AuthenticateUserWithoutSessionChecking(string userName, byte[] token)
        {
            if (!this.connectedUsers.ContainsKey(userName))
                throw new AuthenticateUserException(Invariant($"User '{userName}' is not connected."));

            if (!this.TokenProvider.IsEqualTokens(token, this.connectedUsers[userName].Token))
                throw new AuthenticateUserException("User token is invalid.");
        }

        private static int GetUserSessionTimeoutSecondsSetting()
        {
            const int defaultValue = 120;

            string valueStr = WebConfigurationManager.AppSettings["vfsservice:UserSessionTimeoutSeconds"];

            if (string.IsNullOrWhiteSpace(valueStr))
                return defaultValue;

            try
            {
                int result = XmlConvert.ToInt32(valueStr);
                return result < 0 ? defaultValue : result;
            }
            catch
            {
                return defaultValue;
            }
        }

        private bool IsActualUserSession(string userName)
        {
            DateTime lastActivityTimeUtc = this.connectedUsers[userName].LastActivityTimeUtc;

            TimeSpan userSessionTimeout = TimeSpan.FromTicks(
                TimeSpan.TicksPerSecond *
                GetUserSessionTimeoutSecondsSetting()
            );

            DateTime nowUtc = DateTime.UtcNow;
            return
                nowUtc >= lastActivityTimeUtc &&
                nowUtc - lastActivityTimeUtc <= userSessionTimeout;
        }

        private void AuthenticateUser(string userName, byte[] token)
        {
            AuthenticateUserWithoutSessionChecking(userName, token);
            
            if (!this.IsActualUserSession(userName))
                throw new AuthenticateUserException(Invariant($"'{userName}' user session is not actual or is expired."));
        }

        private static FaultException<AuthorizeFault> CreateAuthorizeFaultException(string userName, string message) =>
            new FaultException<AuthorizeFault>(
                detail: new AuthorizeFault() { UserName = userName },
                reason: message
            );

        private static FaultException<DeauthorizeFault> CreateDeauthorizeFaultException(string userName, string message) =>
            new FaultException<DeauthorizeFault>(
                detail: new DeauthorizeFault() { UserName = userName },
                reason: message
            );

        private static FaultException<FileSystemConsoleFault> CreateFileSystemConsoleFaultException(string userName, string commandLine, string message) =>
            new FaultException<FileSystemConsoleFault>(
                detail: new FileSystemConsoleFault() { UserName = userName, CommandLine = commandLine },
                reason: message
            );

        public AuthorizeResponse Authorize(AuthorizeRequest request)
        {
            if (request is null)
                throw CreateAuthorizeFaultException(null, Invariant($"{nameof(request)} is null."));

            if (request.UserName is null)
                throw CreateAuthorizeFaultException(request.UserName, Invariant($"{nameof(request.UserName)} is null."));

            if (string.IsNullOrEmpty(request.UserName))
                throw CreateAuthorizeFaultException(request.UserName, Invariant($"{nameof(request.UserName)} is empty."));

            request.UserName = request.UserName.Trim();

            if (this.connectedUsers.ContainsKey(request.UserName))
            {
                if (this.IsActualUserSession(request.UserName))
                    throw CreateAuthorizeFaultException(request.UserName, Invariant($"User '{request.UserName}' already connected."));

                this.connectedUsers.Remove(request.UserName);
            }

            byte[] token = this.TokenProvider.GenerateToken();

            this.connectedUsers.Add(request.UserName, new UserSessionInfo(DateTime.UtcNow, token));

            return new AuthorizeResponse() { UserName = request.UserName, Token = token, TotalUsers = this.connectedUsers.Count };
        }

        public DeauthorizeResponse Deauthorize(DeauthorizeRequest request)
        {
            if (request is null)
                throw CreateDeauthorizeFaultException(null, Invariant($"{nameof(request)} is null."));

            if (request.UserName is null)
                throw CreateDeauthorizeFaultException(request.UserName, Invariant($"{nameof(request.UserName)} is null."));

            if (string.IsNullOrEmpty(request.UserName))
                throw CreateDeauthorizeFaultException(request.UserName, Invariant($"{nameof(request.UserName)} is empty."));

            if (request.Token is null)
                throw CreateDeauthorizeFaultException(request.UserName, Invariant($"{nameof(request.Token)} is null."));

            if (request.Token.Length == 0)
                throw CreateDeauthorizeFaultException(request.UserName, Invariant($"{nameof(request.Token)} is empty."));

            request.UserName = request.UserName.Trim();

            try
            {
                this.AuthenticateUserWithoutSessionChecking(request.UserName, request.Token);
            }
            catch (AuthenticateUserException e)
            {
                throw CreateDeauthorizeFaultException(request.UserName, e.Message);
            }

            this.connectedUsers.Remove(request.UserName);

            return new DeauthorizeResponse() { UserName = request.UserName };
        }

        private string ProcessFileSystemConsoleOperation(FileSystemConsoleRequest request, IConsoleCommand<ConsoleCommandCode> command)
        {
            const string defaultResponseMessage = "Command performed successfully.";

            Action<int> checkParameterCount = estimatedCount =>
            {
                if (command.Parameters.Count < estimatedCount)
                    throw CreateFileSystemConsoleFaultException(request.UserName, request.CommandLine, "Command parameters count too small.");
            };

            switch (command.CommandCode)
            {
                case ConsoleCommandCode.ChangeDirectory:
                    {
                        checkParameterCount(1);
                        this.connectedUsers[request.UserName].CurrentDirectory =
                            this.Console.ChangeDirectory(this.connectedUsers[request.UserName].CurrentDirectory, command.Parameters[0]);
                        return Invariant($"New current directory '{this.connectedUsers[request.UserName].CurrentDirectory}' for user '{request.UserName}' successfully set.");
                    }

                case ConsoleCommandCode.CopyTree:
                    {
                        checkParameterCount(2);
                        this.Console.Copy(this.connectedUsers[request.UserName].CurrentDirectory, command.Parameters[0], command.Parameters[1]);
                        return defaultResponseMessage;
                    }

                case ConsoleCommandCode.DeleteFile:
                    {
                        checkParameterCount(1);
                        string fileName = this.Console.DeleteFile(this.connectedUsers[request.UserName].CurrentDirectory, command.Parameters[0]);
                        return Invariant($"File '{fileName}' deleted succesfully.");
                    }

                case ConsoleCommandCode.DeleteTree:
                    {
                        checkParameterCount(1);
                        string directory = this.Console.DeleteTree(this.connectedUsers[request.UserName].CurrentDirectory, command.Parameters[0]);
                        return Invariant($"Tree '{directory}' removed succesfully.");
                    }

                case ConsoleCommandCode.LockFile:
                    {
                        checkParameterCount(1);
                        string fileName = this.Console.LockFile(request.UserName, this.connectedUsers[request.UserName].CurrentDirectory, command.Parameters[0]);
                        return Invariant($"File '{fileName}' locked succesfully by user '{request.UserName}'.");
                    }

                case ConsoleCommandCode.MakeDirectory:
                    {
                        checkParameterCount(1);
                        string directory = this.Console.MakeDirectory(this.connectedUsers[request.UserName].CurrentDirectory, command.Parameters[0]);
                        return Invariant($"Directory '{directory}' created succesfully.");
                    }

                case ConsoleCommandCode.MakeFile:
                    {
                        checkParameterCount(1);
                        string fileName = this.Console.MakeFile(this.connectedUsers[request.UserName].CurrentDirectory, command.Parameters[0]);
                        return Invariant($"File '{fileName}' created succesfully.");
                    }

                case ConsoleCommandCode.MoveTree:
                    {
                        checkParameterCount(2);
                        this.Console.Move(this.connectedUsers[request.UserName].CurrentDirectory, command.Parameters[0], command.Parameters[1]);
                        return defaultResponseMessage;
                    }

                case ConsoleCommandCode.PrintTree:
                    {
                        //checkParameterCount(0);
                        bool printRoot = command.Parameters.Count >= 1 && string.Equals(command.Parameters[0], "Root", StringComparison.OrdinalIgnoreCase);
                        return this.Console.PrintTree(printRoot);
                    }

                case ConsoleCommandCode.RemoveDirectory:
                    {
                        checkParameterCount(1);
                        string directory = this.Console.RemoveDirectory(this.connectedUsers[request.UserName].CurrentDirectory, command.Parameters[0]);
                        return Invariant($"Directory '{directory}' removed succesfully.");
                    }

                case ConsoleCommandCode.UnlockFile:
                    {
                        checkParameterCount(1);
                        string fileName = this.Console.UnlockFile(request.UserName, this.connectedUsers[request.UserName].CurrentDirectory, command.Parameters[0]);
                        return Invariant($"File '{fileName}' unlocked succesfully by user '{request.UserName}'.");
                    }

                default:
                    {
                        throw CreateFileSystemConsoleFaultException(
                            request.UserName,
                            request.CommandLine,
                            Invariant($"Unsupported command ({command.Command}).")
                        );
                    }
            }
        }

        public FileSystemConsoleResponse FileSystemConsole(FileSystemConsoleRequest request)
        {
            if (request is null)
                throw CreateFileSystemConsoleFaultException(null, null, Invariant($"{nameof(request)} is null."));

            if (request.UserName is null)
                throw CreateFileSystemConsoleFaultException(request.UserName, request.CommandLine, Invariant($"{nameof(request.UserName)} is null."));

            if (string.IsNullOrEmpty(request.UserName))
                throw CreateFileSystemConsoleFaultException(request.UserName, request.CommandLine, Invariant($"{nameof(request.UserName)} is empty."));

            if (request.CommandLine is null)
                throw CreateFileSystemConsoleFaultException(request.UserName, request.CommandLine, Invariant($"{nameof(request.CommandLine)} is null."));

            if (string.IsNullOrEmpty(request.CommandLine))
                throw CreateFileSystemConsoleFaultException(request.UserName, request.CommandLine, Invariant($"{nameof(request.CommandLine)} is empty."));

            request.UserName = request.UserName.Trim();

            try
            {
                this.AuthenticateUser(request.UserName, request.Token);
            }
            catch (AuthenticateUserException e)
            {
                throw CreateFileSystemConsoleFaultException(request.UserName, request.CommandLine, e.Message);
            }

            this.connectedUsers[request.UserName].LastActivityTimeUtc = DateTime.UtcNow;

            IConsoleCommand<ConsoleCommandCode> command;
            try
            {
                command = new ConsoleCommand<ConsoleCommandCode>(request.CommandLine, isCaseSensitive: false);
            }
            catch (ArgumentException e)
            {
                throw CreateFileSystemConsoleFaultException(request.UserName, request.CommandLine, Invariant($"Command line is incorrect: {e.Message}."));
            }

            string responseMessage;
            try
            {
                responseMessage = this.ProcessFileSystemConsoleOperation(request, command);
            }
            catch (Exception e)
            {
                throw CreateFileSystemConsoleFaultException(request.UserName, request.CommandLine, e.Message);
            }

            this.GetCallbackChannel().Notify(new FileSystemConsoleNotificationData() { UserName = request.UserName, CommandLine = request.CommandLine });

            return new FileSystemConsoleResponse()
            {
                UserName = request.UserName,
                CurrentDirectory = this.connectedUsers[request.UserName].CurrentDirectory,
                CommandLine = request.CommandLine,
                ResponseMessage = responseMessage
            };

        }
    }

}
