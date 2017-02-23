using System;
using System.Web.Configuration;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Xml;
using VirtualFileSystem.Model;
using static System.FormattableString;

namespace VirtualFileSystem.Service
{

    internal sealed class UserSessionInfo
    {
        private DateTime lastActivityTimeUtc;

        public DateTime LastActivityTimeUtc
        {
            get { return this.lastActivityTimeUtc; }
            set { this.lastActivityTimeUtc = value.ToUniversalTime(); }
        }

        public byte[] Token { get; }

        public string CurrentDirectory { get; set; }

        public UserSessionInfo(DateTime lastActivityTime, byte[] token)
        {
            this.LastActivityTimeUtc = lastActivityTime;
            this.Token = token;
        }
    }

    internal sealed class AuthenticateUserException : Exception
    {
        public AuthenticateUserException(string message) : base(message)
        {
        }
    }

    internal sealed class TokenProvider
    {
        private readonly RandomNumberGenerator tokenGenerator = RandomNumberGenerator.Create();

        public const int TokenLength = 64;

        public byte[] GenerateToken()
        {
            byte[] token = new byte[TokenLength];
            this.tokenGenerator.GetBytes(token);
            return token;
        }

        public static bool IsEqualTokens(byte[] token1, byte[] token2)
        {
            Action<byte[], string> checkToken = (token, tokenName) =>
            {
                if ((object)token == null)
                    throw new ArgumentNullException(paramName: tokenName);

                if (token.Length == 0)
                    throw new ArgumentException(paramName: tokenName, message: "Token is empty.");

                if (token.Length != TokenLength)
                    throw new ArgumentException(paramName: tokenName, message: "Token is an invalid token (token has an invalid length).");
            };

            checkToken(token1, nameof(token1));
            checkToken(token2, nameof(token2));

            for (int i = 0; i < token1.Length; i++)
                if (token1[i] != token2[i])
                    return false;

            return true;
        }
    }

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
        private readonly IVirtualFS vfs = VirtualFSFactory.Default;

        private IVFSServiceCallback Callback => OperationContext.Current.GetCallbackChannel<IVFSServiceCallback>();

        private readonly Dictionary<string, UserSessionInfo> connectedUsers = new Dictionary<string, UserSessionInfo>();

        private const int TokenLength = 64;

        private readonly TokenProvider tokenProvider = new TokenProvider();

        private void AuthenticateUserWithoutSessionChecking(string userName, byte[] token)
        {
            if (!this.connectedUsers.ContainsKey(userName))
                throw new AuthenticateUserException(Invariant($"User '{userName}' is not connected."));

            if (!TokenProvider.IsEqualTokens(token, connectedUsers[userName].Token))
                throw new AuthenticateUserException("User token is invalid.");
        }

        private bool IsActualUserSession(string userName)
        {
            DateTime lastActivityTimeUtc = connectedUsers[userName].LastActivityTimeUtc;

            TimeSpan userSessionTimeout = TimeSpan.FromTicks(
                TimeSpan.TicksPerSecond *
                XmlConvert.ToInt32(WebConfigurationManager.AppSettings["vfsservice:UserSessionTimeoutSeconds"])
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
            if ((object)request == null)
                throw CreateConnectFaultException(null, Invariant($"{nameof(request)} is null."));

            if ((object)request.UserName == null)
                throw CreateConnectFaultException(request.UserName, Invariant($"{nameof(request.UserName)} is null."));

            if (string.IsNullOrEmpty(request.UserName))
                throw CreateConnectFaultException(request.UserName, Invariant($"{nameof(request.UserName)} is empty."));

            request.UserName = request.UserName.Trim();

            if (this.connectedUsers.ContainsKey(request.UserName))
            {
                if (this.IsActualUserSession(request.UserName))
                    throw CreateConnectFaultException(request.UserName, Invariant($"User '{request.UserName}' already connected."));
                else
                    this.connectedUsers.Remove(request.UserName);
            }

            byte[] token = this.tokenProvider.GenerateToken();

            this.connectedUsers.Add(request.UserName, new UserSessionInfo(DateTime.UtcNow, token));

            return new ConnectResponse() { UserName = request.UserName, Token = token, TotalUsers = this.connectedUsers.Count };
        }

        public DisconnectResponse Disconnect(DisconnectRequest request)
        {
            if ((object)request == null)
                throw CreateDisconnectFaultException(null, Invariant($"{nameof(request)} is null."));

            if ((object)request.UserName == null)
                throw CreateDisconnectFaultException(request.UserName, Invariant($"{nameof(request.UserName)} is null."));

            if (string.IsNullOrEmpty(request.UserName))
                throw CreateDisconnectFaultException(request.UserName, Invariant($"{nameof(request.UserName)} is empty."));

            if ((object)request.Token == null)
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

        public FSCommandResponse FSCommand(FSCommandRequest request)
        {
            if ((object)request == null)
                throw CreateFSCommandFaultException(null, null, Invariant($"{nameof(request)} is null."));

            if ((object)request.UserName == null)
                throw CreateFSCommandFaultException(request.UserName, request.CommandLine, Invariant($"{nameof(request.UserName)} is null."));

            if (string.IsNullOrEmpty(request.UserName))
                throw CreateFSCommandFaultException(request.UserName, request.CommandLine, Invariant($"{nameof(request.UserName)} is empty."));

            if ((object)request.CommandLine == null)
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

            ConsoleCommand command;

            try
            {
                command = ConsoleCommand.Parse(request.CommandLine);
            }
            catch (ArgumentException e)
            {
                throw CreateFSCommandFaultException(request.UserName, request.CommandLine, Invariant($"Command line is incorrect: {e.Message}."));
            }

            Action<int> checkParameterCount = estimatedCount =>
            {
                if (command.Parameters.Count < estimatedCount)
                    throw CreateFSCommandFaultException(request.UserName, request.CommandLine, "Command parameters count too small.");
            };

            const string defaultResponseMessage = "Command performed successfully.";
            string responseMessage;

            try
            {
                switch (command.CommandCode)
                {
                    case ConsoleCommandCode.ChangeDirectory:
                        {
                            checkParameterCount(1);
                            this.connectedUsers[request.UserName].CurrentDirectory =
                                vfs.ChangeDirectory(this.connectedUsers[request.UserName].CurrentDirectory, command.Parameters[0]);
                            responseMessage = Invariant($"New current directory '{this.connectedUsers[request.UserName].CurrentDirectory}' for user '{request.UserName}' successfully set.");
                        }
                        break;

                    case ConsoleCommandCode.CopyTree:
                        {
                            checkParameterCount(2);
                            vfs.Copy(this.connectedUsers[request.UserName].CurrentDirectory, command.Parameters[0], command.Parameters[1]);
                            responseMessage = defaultResponseMessage;
                        }
                        break;

                    case ConsoleCommandCode.DeleteFile:
                        {
                            checkParameterCount(1);
                            string fileName = vfs.DeleteFile(this.connectedUsers[request.UserName].CurrentDirectory, command.Parameters[0]);
                            responseMessage = Invariant($"File '{fileName}' deleted succesfully.");
                        }
                        break;

                    case ConsoleCommandCode.DeleteTree:
                        {
                            checkParameterCount(1);
                            string directory = vfs.DeleteTree(this.connectedUsers[request.UserName].CurrentDirectory, command.Parameters[0]);
                            responseMessage = Invariant($"Tree '{directory}' removed succesfully.");
                        }
                        break;

                    case ConsoleCommandCode.LockFile:
                        {
                            checkParameterCount(1);
                            string fileName = vfs.LockFile(request.UserName, this.connectedUsers[request.UserName].CurrentDirectory, command.Parameters[0]);
                            responseMessage = Invariant($"File '{fileName}' locked succesfully by user '{request.UserName}'.");
                        }
                        break;

                    case ConsoleCommandCode.MakeDirectory:
                        {
                            checkParameterCount(1);
                            string directory = vfs.MakeDirectory(this.connectedUsers[request.UserName].CurrentDirectory, command.Parameters[0]);
                            responseMessage = Invariant($"Directory '{directory}' created succesfully.");
                        }
                        break;

                    case ConsoleCommandCode.MakeFile:
                        {
                            checkParameterCount(1);
                            string fileName = vfs.MakeFile(this.connectedUsers[request.UserName].CurrentDirectory, command.Parameters[0]);
                            responseMessage = Invariant($"File '{fileName}' created succesfully.");
                        }
                        break;

                    case ConsoleCommandCode.MoveTree:
                        {
                            checkParameterCount(2);
                            vfs.Move(this.connectedUsers[request.UserName].CurrentDirectory, command.Parameters[0], command.Parameters[1]);
                            responseMessage = defaultResponseMessage;
                        }
                        break;

                    case ConsoleCommandCode.PrintTree:
                        {
                            checkParameterCount(0);
                            responseMessage = vfs.PrintTree();
                            responseMessage = defaultResponseMessage;
                        }
                        break;

                    case ConsoleCommandCode.RemoveDirectory:
                        {
                            checkParameterCount(1);
                            string directory = vfs.RemoveDirectory(this.connectedUsers[request.UserName].CurrentDirectory, command.Parameters[0]);
                            responseMessage = Invariant($"Directory '{directory}' removed succesfully.");
                        }
                        break;

                    case ConsoleCommandCode.UnlockFile:
                        {
                            checkParameterCount(1);
                            string fileName = vfs.UnlockFile(request.UserName, this.connectedUsers[request.UserName].CurrentDirectory, command.Parameters[0]);
                            responseMessage = Invariant($"File '{fileName}' unlocked succesfully by user '{request.UserName}'.");
                        }
                        break;

                    default:
                        throw CreateFSCommandFaultException(request.UserName, request.CommandLine, Invariant($"Unsupported command code ({command.CommandCode})."));
                }
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
