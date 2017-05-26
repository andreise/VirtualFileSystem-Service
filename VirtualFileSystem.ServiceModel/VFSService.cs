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

        private readonly Dictionary<string, UserSessionInfo> Users = new Dictionary<string, UserSessionInfo>(UserNameComparerProvider.Default);

        private void AuthenticateUserWithoutSessionChecking(string userName, byte[] token)
        {
            if (!this.Users.ContainsKey(userName))
                throw new AuthenticateUserException(Invariant($"User '{userName}' is not connected."));

            if (!this.TokenProvider.IsEqualTokens(token, this.Users[userName].Token))
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
            DateTime lastActivityTimeUtc = this.Users[userName].LastActivityTimeUtc;

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

        private static FaultException<CommandFault> CreateCommandFaultException(string userName, string commandLine, string message) =>
            new FaultException<CommandFault>(
                detail: new CommandFault() { UserName = userName, CommandLine = commandLine },
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

            if (this.Users.ContainsKey(request.UserName))
            {
                if (this.IsActualUserSession(request.UserName))
                    throw CreateAuthorizeFaultException(request.UserName, Invariant($"User '{request.UserName}' already connected."));

                this.Users.Remove(request.UserName);
            }

            byte[] token = this.TokenProvider.GenerateToken();

            this.Users.Add(request.UserName, new UserSessionInfo(DateTime.UtcNow, token));

            return new AuthorizeResponse() { UserName = request.UserName, Token = token, TotalUsers = this.Users.Count };
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

            this.Users.Remove(request.UserName);

            return new DeauthorizeResponse() { UserName = request.UserName };
        }

        private string PerformCommandHelper(CommandRequest request, IConsoleCommand<ConsoleCommandCode> command)
        {
            const string defaultResponseMessage = "Command performed successfully.";

            void ValidateParameterCount(int estimatedCount)
            {
                if (command.Parameters.Count < estimatedCount)
                    throw CreateCommandFaultException(request.UserName, request.CommandLine, "Command parameters count too small.");
            };

            switch (command.CommandCode)
            {
                case ConsoleCommandCode.ChangeDirectory:
                    {
                        ValidateParameterCount(1);
                        this.Users[request.UserName].CurrentDirectory =
                            this.Console.ChangeDirectory(this.Users[request.UserName].CurrentDirectory, command.Parameters[0]);
                        return Invariant($"New current directory '{this.Users[request.UserName].CurrentDirectory}' for user '{request.UserName}' successfully set.");
                    }

                case ConsoleCommandCode.CopyTree:
                    {
                        ValidateParameterCount(2);
                        this.Console.Copy(this.Users[request.UserName].CurrentDirectory, command.Parameters[0], command.Parameters[1]);
                        return defaultResponseMessage;
                    }

                case ConsoleCommandCode.DeleteFile:
                    {
                        ValidateParameterCount(1);
                        string fileName = this.Console.DeleteFile(this.Users[request.UserName].CurrentDirectory, command.Parameters[0]);
                        return Invariant($"File '{fileName}' deleted succesfully.");
                    }

                case ConsoleCommandCode.DeleteTree:
                    {
                        ValidateParameterCount(1);
                        string directory = this.Console.DeleteTree(this.Users[request.UserName].CurrentDirectory, command.Parameters[0]);
                        return Invariant($"Tree '{directory}' removed succesfully.");
                    }

                case ConsoleCommandCode.LockFile:
                    {
                        ValidateParameterCount(1);
                        string fileName = this.Console.LockFile(request.UserName, this.Users[request.UserName].CurrentDirectory, command.Parameters[0]);
                        return Invariant($"File '{fileName}' locked succesfully by user '{request.UserName}'.");
                    }

                case ConsoleCommandCode.MakeDirectory:
                    {
                        ValidateParameterCount(1);
                        string directory = this.Console.MakeDirectory(this.Users[request.UserName].CurrentDirectory, command.Parameters[0]);
                        return Invariant($"Directory '{directory}' created succesfully.");
                    }

                case ConsoleCommandCode.MakeFile:
                    {
                        ValidateParameterCount(1);
                        string fileName = this.Console.MakeFile(this.Users[request.UserName].CurrentDirectory, command.Parameters[0]);
                        return Invariant($"File '{fileName}' created succesfully.");
                    }

                case ConsoleCommandCode.MoveTree:
                    {
                        ValidateParameterCount(2);
                        this.Console.Move(this.Users[request.UserName].CurrentDirectory, command.Parameters[0], command.Parameters[1]);
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
                        ValidateParameterCount(1);
                        string directory = this.Console.RemoveDirectory(this.Users[request.UserName].CurrentDirectory, command.Parameters[0]);
                        return Invariant($"Directory '{directory}' removed succesfully.");
                    }

                case ConsoleCommandCode.UnlockFile:
                    {
                        ValidateParameterCount(1);
                        string fileName = this.Console.UnlockFile(request.UserName, this.Users[request.UserName].CurrentDirectory, command.Parameters[0]);
                        return Invariant($"File '{fileName}' unlocked succesfully by user '{request.UserName}'.");
                    }

                default:
                    {
                        throw CreateCommandFaultException(
                            request.UserName,
                            request.CommandLine,
                            Invariant($"Unsupported command ({command.Command}).")
                        );
                    }
            }
        }

        public CommandResponse PerformCommand(CommandRequest request)
        {
            if (request is null)
                throw CreateCommandFaultException(null, null, Invariant($"{nameof(request)} is null."));

            if (request.UserName is null)
                throw CreateCommandFaultException(request.UserName, request.CommandLine, Invariant($"{nameof(request.UserName)} is null."));

            if (string.IsNullOrEmpty(request.UserName))
                throw CreateCommandFaultException(request.UserName, request.CommandLine, Invariant($"{nameof(request.UserName)} is empty."));

            if (request.CommandLine is null)
                throw CreateCommandFaultException(request.UserName, request.CommandLine, Invariant($"{nameof(request.CommandLine)} is null."));

            if (string.IsNullOrEmpty(request.CommandLine))
                throw CreateCommandFaultException(request.UserName, request.CommandLine, Invariant($"{nameof(request.CommandLine)} is empty."));

            request.UserName = request.UserName.Trim();

            try
            {
                this.AuthenticateUser(request.UserName, request.Token);
            }
            catch (AuthenticateUserException e)
            {
                throw CreateCommandFaultException(request.UserName, request.CommandLine, e.Message);
            }

            this.Users[request.UserName].LastActivityTimeUtc = DateTime.UtcNow;

            IConsoleCommand<ConsoleCommandCode> command;
            try
            {
                command = new ConsoleCommand<ConsoleCommandCode>(request.CommandLine, isCaseSensitive: false);
            }
            catch (ArgumentException e)
            {
                throw CreateCommandFaultException(request.UserName, request.CommandLine, Invariant($"Command line is incorrect: {e.Message}."));
            }

            CommandPerformedData CreateCommandPerformedData(bool isSuccess, string message) => new CommandPerformedData()
            {
                UserName = request.UserName,
                CommandLine = request.CommandLine,
                IsSuccess = isSuccess,
                ResponseMessage = message
            };

            string responseMessage;
            try
            {
                responseMessage = this.PerformCommandHelper(request, command);
            }
            catch (Exception e)
            {
                string message = e.Message;
                try
                {
                    this.GetCallbackChannel().OnCommandPerformed(CreateCommandPerformedData(isSuccess: false, message: message));
                }
                finally
                {
                    throw CreateCommandFaultException(request.UserName, request.CommandLine, message);
                }
            }

            this.GetCallbackChannel().OnCommandPerformed(CreateCommandPerformedData(isSuccess: true, message: responseMessage));

            return new CommandResponse()
            {
                UserName = request.UserName,
                CurrentDirectory = this.Users[request.UserName].CurrentDirectory,
                CommandLine = request.CommandLine,
                ResponseMessage = responseMessage
            };

        }
    }

}
