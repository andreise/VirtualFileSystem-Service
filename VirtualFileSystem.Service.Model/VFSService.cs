using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Web.Configuration;
using System.Xml;
using VFSCommon;
using VirtualFileSystem.Model;
using static System.FormattableString;

namespace VirtualFileSystem.Service.Model
{
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
        private IVFSConsole VFSConsole => VFSProvider.Default;

        private IVFSServiceCallback Callback => OperationContext.Current.GetCallbackChannel<IVFSServiceCallback>();

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

        private static FaultException<ConnectFault> CreateConnectFaultException(string userName, string message) =>
            new FaultException<ConnectFault>(
                detail: new ConnectFault() { UserName = userName, FaultMessage = message },
                reason: message
            );

        private static FaultException<DisconnectFault> CreateDisconnectFaultException(string userName, string message) =>
            new FaultException<DisconnectFault>(
                detail: new DisconnectFault() { UserName = userName, FaultMessage = message },
                reason: message
            );

        private static FaultException<FSCommandFault> CreateFSCommandFaultException(string userName, string commandLine, string message) =>
            new FaultException<FSCommandFault>(
                detail: new FSCommandFault() { UserName = userName, CommandLine = commandLine, FaultMessage = message },
                reason: message
            );

        public ConnectResponse Connect(ConnectRequest request)
        {
            if (request is null)
                throw CreateConnectFaultException(null, Invariant($"{nameof(request)} is null."));

            if (request.UserName is null)
                throw CreateConnectFaultException(request.UserName, Invariant($"{nameof(request.UserName)} is null."));

            if (string.IsNullOrEmpty(request.UserName))
                throw CreateConnectFaultException(request.UserName, Invariant($"{nameof(request.UserName)} is empty."));

            request.UserName = request.UserName.Trim();

            if (this.connectedUsers.ContainsKey(request.UserName))
            {
                if (this.IsActualUserSession(request.UserName))
                    throw CreateConnectFaultException(request.UserName, Invariant($"User '{request.UserName}' already connected."));

                this.connectedUsers.Remove(request.UserName);
            }

            byte[] token = this.TokenProvider.GenerateToken();

            this.connectedUsers.Add(request.UserName, new UserSessionInfo(DateTime.UtcNow, token));

            return new ConnectResponse() { UserName = request.UserName, Token = token, TotalUsers = this.connectedUsers.Count };
        }

        public DisconnectResponse Disconnect(DisconnectRequest request)
        {
            if (request is null)
                throw CreateDisconnectFaultException(null, Invariant($"{nameof(request)} is null."));

            if (request.UserName is null)
                throw CreateDisconnectFaultException(request.UserName, Invariant($"{nameof(request.UserName)} is null."));

            if (string.IsNullOrEmpty(request.UserName))
                throw CreateDisconnectFaultException(request.UserName, Invariant($"{nameof(request.UserName)} is empty."));

            if (request.Token is null)
                throw CreateDisconnectFaultException(request.UserName, Invariant($"{nameof(request.Token)} is null."));

            if (request.Token.Length == 0)
                throw CreateDisconnectFaultException(request.UserName, Invariant($"{nameof(request.Token)} is empty."));

            request.UserName = request.UserName.Trim();

            try
            {
                this.AuthenticateUserWithoutSessionChecking(request.UserName, request.Token);
            }
            catch (AuthenticateUserException e)
            {
                throw CreateDisconnectFaultException(request.UserName, e.Message);
            }

            this.connectedUsers.Remove(request.UserName);

            return new DisconnectResponse() { UserName = request.UserName };
        }

        private string ProcessRequestCommand(FSCommandRequest request, ConsoleCommand<ConsoleCommandCode> command)
        {
            const string defaultResponseMessage = "Command performed successfully.";

            Action<int> checkParameterCount = estimatedCount =>
            {
                if (command.Parameters.Count < estimatedCount)
                    throw CreateFSCommandFaultException(request.UserName, request.CommandLine, "Command parameters count too small.");
            };

            switch (command.CommandCode)
            {
                case ConsoleCommandCode.ChangeDirectory:
                    {
                        checkParameterCount(1);
                        this.connectedUsers[request.UserName].CurrentDirectory =
                            this.VFSConsole.ChangeDirectory(this.connectedUsers[request.UserName].CurrentDirectory, command.Parameters[0]);
                        return Invariant($"New current directory '{this.connectedUsers[request.UserName].CurrentDirectory}' for user '{request.UserName}' successfully set.");
                    }

                case ConsoleCommandCode.CopyTree:
                    {
                        checkParameterCount(2);
                        this.VFSConsole.Copy(this.connectedUsers[request.UserName].CurrentDirectory, command.Parameters[0], command.Parameters[1]);
                        return defaultResponseMessage;
                    }

                case ConsoleCommandCode.DeleteFile:
                    {
                        checkParameterCount(1);
                        string fileName = this.VFSConsole.DeleteFile(this.connectedUsers[request.UserName].CurrentDirectory, command.Parameters[0]);
                        return Invariant($"File '{fileName}' deleted succesfully.");
                    }

                case ConsoleCommandCode.DeleteTree:
                    {
                        checkParameterCount(1);
                        string directory = this.VFSConsole.DeleteTree(this.connectedUsers[request.UserName].CurrentDirectory, command.Parameters[0]);
                        return Invariant($"Tree '{directory}' removed succesfully.");
                    }

                case ConsoleCommandCode.LockFile:
                    {
                        checkParameterCount(1);
                        string fileName = this.VFSConsole.LockFile(request.UserName, this.connectedUsers[request.UserName].CurrentDirectory, command.Parameters[0]);
                        return Invariant($"File '{fileName}' locked succesfully by user '{request.UserName}'.");
                    }

                case ConsoleCommandCode.MakeDirectory:
                    {
                        checkParameterCount(1);
                        string directory = this.VFSConsole.MakeDirectory(this.connectedUsers[request.UserName].CurrentDirectory, command.Parameters[0]);
                        return Invariant($"Directory '{directory}' created succesfully.");
                    }

                case ConsoleCommandCode.MakeFile:
                    {
                        checkParameterCount(1);
                        string fileName = this.VFSConsole.MakeFile(this.connectedUsers[request.UserName].CurrentDirectory, command.Parameters[0]);
                        return Invariant($"File '{fileName}' created succesfully.");
                    }

                case ConsoleCommandCode.MoveTree:
                    {
                        checkParameterCount(2);
                        this.VFSConsole.Move(this.connectedUsers[request.UserName].CurrentDirectory, command.Parameters[0], command.Parameters[1]);
                        return defaultResponseMessage;
                    }

                case ConsoleCommandCode.PrintTree:
                    {
                        checkParameterCount(0);
                        return this.VFSConsole.PrintTree();
                    }

                case ConsoleCommandCode.RemoveDirectory:
                    {
                        checkParameterCount(1);
                        string directory = this.VFSConsole.RemoveDirectory(this.connectedUsers[request.UserName].CurrentDirectory, command.Parameters[0]);
                        return Invariant($"Directory '{directory}' removed succesfully.");
                    }

                case ConsoleCommandCode.UnlockFile:
                    {
                        checkParameterCount(1);
                        string fileName = this.VFSConsole.UnlockFile(request.UserName, this.connectedUsers[request.UserName].CurrentDirectory, command.Parameters[0]);
                        return Invariant($"File '{fileName}' unlocked succesfully by user '{request.UserName}'.");
                    }

                default:
                    {
                        throw CreateFSCommandFaultException(
                            request.UserName,
                            request.CommandLine,
                            Invariant($"Unsupported command ({command.Command}).")
                        );
                    }
            }
        }

        public FSCommandResponse FSCommand(FSCommandRequest request)
        {
            if (request is null)
                throw CreateFSCommandFaultException(null, null, Invariant($"{nameof(request)} is null."));

            if (request.UserName is null)
                throw CreateFSCommandFaultException(request.UserName, request.CommandLine, Invariant($"{nameof(request.UserName)} is null."));

            if (string.IsNullOrEmpty(request.UserName))
                throw CreateFSCommandFaultException(request.UserName, request.CommandLine, Invariant($"{nameof(request.UserName)} is empty."));

            if (request.CommandLine is null)
                throw CreateFSCommandFaultException(request.UserName, request.CommandLine, Invariant($"{nameof(request.CommandLine)} is null."));

            if (string.IsNullOrEmpty(request.CommandLine))
                throw CreateFSCommandFaultException(request.UserName, request.CommandLine, Invariant($"{nameof(request.CommandLine)} is empty."));

            request.UserName = request.UserName.Trim();

            try
            {
                this.AuthenticateUser(request.UserName, request.Token);
            }
            catch (AuthenticateUserException e)
            {
                throw CreateFSCommandFaultException(request.UserName, request.CommandLine, e.Message);
            }

            this.connectedUsers[request.UserName].LastActivityTimeUtc = DateTime.UtcNow;

            ConsoleCommand<ConsoleCommandCode> command;
            try
            {
                command = ConsoleCommand<ConsoleCommandCode>.Parse(request.CommandLine, isCaseSensitive: false);
            }
            catch (ArgumentException e)
            {
                throw CreateFSCommandFaultException(request.UserName, request.CommandLine, Invariant($"Command line is incorrect: {e.Message}."));
            }

            string responseMessage;
            try
            {
                responseMessage = this.ProcessRequestCommand(request, command);
            }
            catch (Exception e)
            {
                throw CreateFSCommandFaultException(request.UserName, request.CommandLine, e.Message);
            }

            this.Callback.FileSystemChangedNotify(new FileSystemChangedData() { UserName = request.UserName, CommandLine = request.CommandLine });

            return new FSCommandResponse()
            {
                UserName = request.UserName,
                CurrentDirectory = this.connectedUsers[request.UserName].CurrentDirectory,
                CommandLine = request.CommandLine,
                ResponseMessage = responseMessage
            };

        }
    }

}
