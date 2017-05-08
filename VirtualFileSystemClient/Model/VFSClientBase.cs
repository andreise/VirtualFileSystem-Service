using System;
using System.ServiceModel;
using System.Threading.Tasks;
using VirtualFileSystem.Common;
using static System.FormattableString;

namespace VirtualFileSystemClient.Model
{
    using VFSServiceReference;

    internal abstract class VFSClientBase : VFSClientBase<ConnectFault, DisconnectFault, FSCommandFault>
    {

        public VFSClientBase(Action<string> output) : base(output)
        {
        }

        protected abstract VFSServiceClient Service { get; }

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
                throw new FaultException<ConnectFault>(
                    new ConnectFault()
                    {
                        FaultMessage = "Server returned null response.",
                        UserName = userName
                    }
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
                throw new FaultException<DisconnectFault>(
                    new DisconnectFault()
                    {
                        FaultMessage = "Server returned null response.",
                        UserName = this.User.Credentials.UserName
                    }
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
                throw new FaultException<FSCommandFault>(
                    new FSCommandFault()
                    {
                        FaultMessage = "Server returned null response.",
                        UserName = this.User.Credentials.UserName,
                        CommandLine = command.CommandLine
                    }
                );
            }

            this.Output(response.ResponseMessage);
        }

    }

}
