using System;
using System.Threading.Tasks;
using static System.FormattableString;

namespace VirtualFileSystemClient.Model.Common
{
    using Security;
    using VirtualFileSystem.Common;

    public abstract class ClientBase<TAuthorizeException, TDeauthorizeException, TFileSystemConsoleException>
        where TAuthorizeException : Exception
        where TDeauthorizeException : Exception
        where TFileSystemConsoleException : Exception
    {

        protected static bool EqualUserNames(string name1, string name2) => UserNameComparerProvider.Default.Equals(name1, name2);

        private bool alreadyRun;

        private readonly object runLock = new object();

        protected readonly User User = new User();

        protected readonly Func<string> Input;

        protected readonly Action<string> Output;

        public ClientBase(Func<string> input, Action<string> output)
        {
            if (input is null)
                throw new ArgumentNullException(nameof(input));

            if (output is null)
                throw new ArgumentNullException(nameof(output));

            this.Input = input;
            this.Output = output;
        }

        protected async Task ProcessOperation<TException>(Func<Task> handler) where TException : Exception
        {
            try
            {
                await handler();
            }
            catch (TException e)
            {
                this.Output(e.Message);
            }
            catch (AggregateException e) when (e.InnerException is TException)
            {
                this.Output(e.InnerException.Message);
            }
        }

        protected abstract Task ProcessAuthorizeOperationHandler(string userName);

        protected abstract Task ProcessDeauthorizeOperationHandler();

        protected abstract Task ProcessFileSystemConsoleOperationHandler(IConsoleCommand<ConsoleCommandCode> command);

        protected abstract void CloseService();

        private async Task ProcessAuthorizeOperation(IConsoleCommand<ConsoleCommandCode> command)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));

            if (command.CommandCode != ConsoleCommandCode.Connect)
                throw new ArgumentException(
                    paramName: nameof(command),
                    message: Invariant($"Command is not the '{nameof(ConsoleCommandCode.Connect)}' command.")
                );

            string userName;

            if (command.Parameters.Count == 0 || string.IsNullOrWhiteSpace(userName = command.Parameters[0]))
            {
                this.Output("User name not specified.");
                return;
            }

            if (!(this.User.Credentials is null))
            {
                if (!EqualUserNames(this.User.Credentials.UserName, userName))
                {
                    this.Output(Invariant($"Please disconnect current user ('{this.User.Credentials.UserName}') before connect new user."));
                    return;
                }
            }

            await this.ProcessOperation<TAuthorizeException>(async () => await this.ProcessAuthorizeOperationHandler(userName));
        }

        protected async Task ProcessDeauthorizeOperation()
        {
            if (this.User.Credentials is null)
            {
                this.Output("Current user is undefined.");
                return;
            }

            await this.ProcessOperation<TDeauthorizeException>(this.ProcessDeauthorizeOperationHandler);
        }

        protected async Task ProcessFileSystemConsoleOperation(IConsoleCommand<ConsoleCommandCode> command)
        {
            if (command is null)
                throw new ArgumentNullException(paramName: nameof(command));

            if (!(command.CommandCode is null))
                throw new ArgumentException(paramName: nameof(command), message: "Command is not a file system command.");

            if (this.User.Credentials is null)
            {
                this.Output("Please connect to the host before sending to it any other commands.");
                return;
            }

            await this.ProcessOperation<TFileSystemConsoleException>(async () => await this.ProcessFileSystemConsoleOperationHandler(command));
        }

        public async Task Run()
        {
            lock (this.runLock)
            {
                if (this.alreadyRun)
                    throw new InvalidOperationException("Client instance once has already been launched.");

                this.alreadyRun = true;
            }

            this.Output("Virtual File System Client");
            this.Output(Invariant($"Connect to host specified in the endpoint and send commands to the file system, or type '{nameof(ConsoleCommandCode.Quit)}' or '{nameof(ConsoleCommandCode.Exit)}' to exit."));
            this.Output(Invariant($"Type '{ConsoleCommandCode.Connect} UserName'..."));

            IConsoleCommand<ConsoleCommandCode> command;

            IConsoleCommand<ConsoleCommandCode> ReadCommand() => ConsoleCommandParser.TryParse<ConsoleCommandCode>(this.Input(), isCaseSensitive: false);

            try
            {
                while (
                    !((command = ReadCommand()) is null) && command.CommandCode != ConsoleCommandCode.Exit
                )
                {
                    if (string.IsNullOrWhiteSpace(command.CommandLine))
                        continue;

                    switch (command.CommandCode)
                    {
                        case ConsoleCommandCode.Connect:
                            await this.ProcessAuthorizeOperation(command);
                            break;

                        case ConsoleCommandCode.Disconnect:
                            await this.ProcessDeauthorizeOperation();
                            break;

                        default:
                            await this.ProcessFileSystemConsoleOperation(command);
                            break;
                    }
                } // while
            }
            finally
            {
                this.CloseService();
            }
        }

    }

}
