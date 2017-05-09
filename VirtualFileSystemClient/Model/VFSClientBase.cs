using System;
using System.ServiceModel;
using System.Threading.Tasks;
using static System.FormattableString;

namespace VirtualFileSystemClient.Model
{
    using VirtualFileSystem.Common;
    using VFSServiceReference;

    internal abstract class VFSClientBase : VFSClientBase<ConnectFault, DisconnectFault, FSCommandFault>
    {

        private void HandleCallback(FileSystemChangedData data)
        {
            if (data is null || data.UserName is null)
                return;

            if (this.User.Credentials is null)
                return;

            if (EqualUserNames(data.UserName, this.User.Credentials.UserName))
                return;

            this.Output(Invariant($"User '{data.UserName}' performed command: {data.CommandLine}"));
        }

        protected readonly VFSServiceClient Service;

        public VFSClientBase(Action<string> output) : base(output)
        {
            this.Service = new VFSServiceClient(
                new InstanceContext(new VFSServiceCallbackHandler(this.HandleCallback))
            );
        }

        private const string ServerReturnedNullResponseMessage = "Server returned null response.";

        protected override async Task ConnectCommandHandler(string userName)
        {
            var response = await this.Service.ConnectAsync(
                new ConnectRequest()
                {
                    UserName = userName
                }
            );

            if (response is null)
            {
                string message = ServerReturnedNullResponseMessage;
                throw new FaultException<ConnectFault>(
                    new ConnectFault()
                    {
                        FaultMessage = message,
                        UserName = userName
                    },
                    message
                );
            }

            this.User.SetCredentials(userName, response.Token);

            this.Output(Invariant($"User '{response.UserName}' connected successfully."));
            this.Output(Invariant($"Total users: {response.TotalUsers}."));
        }

        protected override async Task DisconnectCommandHandler()
        {
            var response = await this.Service.DisconnectAsync(
                new DisconnectRequest()
                {
                    UserName = this.User.Credentials.UserName,
                    Token = this.User.Credentials.Token
                }
            );

            if (response is null)
            {
                string message = ServerReturnedNullResponseMessage;
                throw new FaultException<DisconnectFault>(
                    new DisconnectFault()
                    {
                        FaultMessage = message,
                        UserName = this.User.Credentials.UserName
                    },
                    message
                );
            }

            this.User.ResetCredentials();

            this.Output(Invariant($"User '{response.UserName}' disconnected."));
        }

        protected override async Task VFSCommandHandler(IConsoleCommand<ConsoleCommandCode> command)
        {
            var response = await this.Service.FSCommandAsync(
                new FSCommandRequest()
                {
                    UserName = this.User.Credentials.UserName,
                    Token = this.User.Credentials.Token,
                    CommandLine = command.CommandLine
                }
            );

            if (response is null)
            {
                string message = ServerReturnedNullResponseMessage;
                throw new FaultException<FSCommandFault>(
                    new FSCommandFault()
                    {
                        FaultMessage = message,
                        UserName = this.User.Credentials.UserName,
                        CommandLine = command.CommandLine
                    },
                    message
                );
            }

            this.Output(response.ResponseMessage);
        }

    }

}
