using System;
using System.ServiceModel;
using System.Threading.Tasks;
using static System.FormattableString;

namespace VirtualFileSystemClient.Model
{
    using VirtualFileSystem.Common.Console;
    using VirtualFileSystemServiceReference;
    using Common;

    public sealed class Client : ClientBase<AuthorizeFault, DeauthorizeFault, FileSystemConsoleFault>
    {

        private const string ServerReturnedNullResponseMessage = "Server returned null response.";

        private bool alreadyRun;

        private readonly object runLock = new object();

        private readonly VFSServiceClient Service;

        private void HandleCallback(FileSystemConsoleNotificationData data)
        {
            if (data is null || data.UserName is null)
                return;

            if (this.User.Credentials is null)
                return;

            if (UserNameComparer.Equals(data.UserName, this.User.Credentials.UserName))
                return;

            this.Output(Invariant($"User '{data.UserName}' performed the command: {data.CommandLine}"));
        }

        public Client(Func<string> input, Action<string> output) : base(input, output)
        {
            this.Service = new VFSServiceClient(
                new InstanceContext(new VFSServiceCallbackHandler(this.HandleCallback))
            );
        }

        public Client() : this(Console.ReadLine, Console.WriteLine)
        {
        }

        protected override async Task ProcessAuthorizeOperationHandler(string userName)
        {
            var response = await this.Service.AuthorizeAsync(
                new AuthorizeRequest()
                {
                    UserName = userName
                }
            );

            if (response is null)
            {
                string message = ServerReturnedNullResponseMessage;
                throw new FaultException<AuthorizeFault>(
                    new AuthorizeFault() { UserName = userName },
                    message
                );
            }

            this.User.SetCredentials(userName, response.Token);

            this.Output(Invariant($"User '{response.UserName}' connected successfully."));
            this.Output(Invariant($"Total users: {response.TotalUsers}."));
        }

        protected override async Task ProcessDeauthorizeOperationHandler()
        {
            var response = await this.Service.DeauthorizeAsync(
                new DeauthorizeRequest()
                {
                    UserName = this.User.Credentials.UserName,
                    Token = this.User.Credentials.Token
                }
            );

            if (response is null)
            {
                string message = ServerReturnedNullResponseMessage;
                throw new FaultException<DeauthorizeFault>(
                    new DeauthorizeFault() { UserName = this.User.Credentials.UserName },
                    message
                );
            }

            this.User.ResetCredentials();

            this.Output(Invariant($"User '{response.UserName}' disconnected."));
        }

        protected override async Task ProcessFileSystemConsoleOperationHandler(IConsoleCommand<ConsoleCommandCode> command)
        {
            var response = await this.Service.FileSystemConsoleAsync(
                new FileSystemConsoleRequest()
                {
                    UserName = this.User.Credentials.UserName,
                    Token = this.User.Credentials.Token,
                    CommandLine = command.CommandLine
                }
            );

            if (response is null)
            {
                string message = ServerReturnedNullResponseMessage;
                throw new FaultException<FileSystemConsoleFault>(
                    new FileSystemConsoleFault()
                    {
                        UserName = this.User.Credentials.UserName,
                        CommandLine = command.CommandLine
                    },
                    message
                );
            }

            this.Output(response.ResponseMessage);
        }

        public override async Task Run()
        {
            lock (this.runLock)
            {
                if (this.alreadyRun)
                    throw new InvalidOperationException("Client instance once has already been launched.");

                this.alreadyRun = true;
            }

            try
            {
                await base.Run();
            }
            finally
            {
                this.Service.Abort(); // async closing the service
            }
        }

    }

}
