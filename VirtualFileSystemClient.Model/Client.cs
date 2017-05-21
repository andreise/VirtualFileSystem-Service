using System;
using System.ServiceModel;
using System.Threading.Tasks;
using static System.FormattableString;

namespace VirtualFileSystemClient.Model
{
    using VirtualFileSystem.Common.Console;
    using VirtualFileSystemServiceReference;
    using Common;

    public sealed class Client : ClientBase<AuthorizeFault, DeauthorizeFault, CommandFault>
    {

        private const string ServerReturnedNullResponseMessage = "Server returned null response.";

        private bool alreadyRun;

        private readonly object runLock = new object();

        private readonly VFSServiceClient Service;

        private void CommandPerformedHandler(CommandPerformedData data)
        {
            if (data is null || data.UserName is null)
                return;

            if (this.User.Credentials is null)
                return;

            if (UserNameComparer.Equals(data.UserName, this.User.Credentials.UserName))
                return;

            string successDescription = data.IsSuccess ? "successfully" : "unsuccessfully";
            string responseTitle = data.IsSuccess ? "Response" : "Error";

            this.WriteLine(null);
            this.WriteLine(Invariant($"User '{data.UserName}' {successDescription} performed the command: {data.CommandLine}"));
            this.WriteLine(Invariant($"{responseTitle} message: {data.ResponseMessage}"));
            this.WriteLine(null);
        }

        public Client(Func<string> readLine, Action<string> writeLine) : base(readLine, writeLine)
        {
            this.Service = new VFSServiceClient(
                new InstanceContext(new VFSServiceCallbackHandler(commandPerformedHandler: this.CommandPerformedHandler))
            );
        }

        public Client() : this(Console.ReadLine, Console.WriteLine)
        {
        }

        protected override async Task AuthorizeHandlerAsync(string userName)
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

            this.WriteLine(Invariant($"User '{response.UserName}' connected successfully."));
            this.WriteLine(Invariant($"Total users: {response.TotalUsers}."));
        }

        protected override async Task DeauthorizeHandlerAsync()
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

            this.WriteLine(Invariant($"User '{response.UserName}' disconnected."));
        }

        protected override async Task FileSystemConsoleHandlerAsync(IConsoleCommand<ConsoleCommandCode> command)
        {
            var response = await this.Service.PerformCommandAsync(
                new CommandRequest()
                {
                    UserName = this.User.Credentials.UserName,
                    Token = this.User.Credentials.Token,
                    CommandLine = command.CommandLine
                }
            );

            if (response is null)
            {
                string message = ServerReturnedNullResponseMessage;
                throw new FaultException<CommandFault>(
                    new CommandFault()
                    {
                        UserName = this.User.Credentials.UserName,
                        CommandLine = command.CommandLine
                    },
                    message
                );
            }

            this.WriteLine(response.ResponseMessage);
        }

        public override async Task RunAsync()
        {
            lock (this.runLock)
            {
                if (this.alreadyRun)
                    throw new InvalidOperationException("Client instance once has already been launched.");

                this.alreadyRun = true;
            }

            try
            {
                await base.RunAsync();
            }
            finally
            {
                this.Service.Abort(); // async closing the service
            }
        }

    }

}
