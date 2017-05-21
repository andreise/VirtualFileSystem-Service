using System;
using System.Threading.Tasks;
using static System.FormattableString;

namespace VirtualFileSystemClient.Model.Common
{
    using Security;
    using VirtualFileSystem.Common.Console;
    using VirtualFileSystem.Common.Security;

    public abstract class ClientBase<TAuthorizeException, TDeauthorizeException, TFileSystemConsoleException>
        where TAuthorizeException : Exception
        where TDeauthorizeException : Exception
        where TFileSystemConsoleException : Exception
    {

        protected static StringComparer UserNameComparer => UserNameComparerProvider.Default;

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

        protected async Task ProcessOperationAsync<TException>(Func<Task> handlerAsync) where TException : Exception
        {
            try
            {
                await handlerAsync();
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

        protected abstract Task AuthorizeHandlerAsync(string userName);

        protected abstract Task DeauthorizeHandlerAsync();

        protected abstract Task FileSystemConsoleHandlerAsync(IConsoleCommand<ConsoleCommandCode> command);

        private async Task AuthorizeAsync(IConsoleCommand<ConsoleCommandCode> command)
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
                if (!UserNameComparer.Equals(this.User.Credentials.UserName, userName))
                {
                    this.Output(Invariant($"Please disconnect current user ('{this.User.Credentials.UserName}') before connect new user."));
                    return;
                }
            }

            await this.ProcessOperationAsync<TAuthorizeException>(async () => await this.AuthorizeHandlerAsync(userName));
        }

        protected async Task DeauthorizeAsync()
        {
            if (this.User.Credentials is null)
            {
                this.Output("Current user is undefined.");
                return;
            }

            await this.ProcessOperationAsync<TDeauthorizeException>(this.DeauthorizeHandlerAsync);
        }

        protected async Task FileSystemConsoleAsync(IConsoleCommand<ConsoleCommandCode> command)
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

            await this.ProcessOperationAsync<TFileSystemConsoleException>(async () => await this.FileSystemConsoleHandlerAsync(command));
        }

        public virtual async Task RunAsync()
        {
            this.Output("Virtual File System Client");
            this.Output(Invariant($"Connect to host specified in the endpoint and send commands to the file system, or type '{nameof(ConsoleCommandCode.Quit)}' or '{nameof(ConsoleCommandCode.Exit)}' to exit."));
            this.Output(Invariant($"Type '{ConsoleCommandCode.Connect} UserName'..."));

            IConsoleCommand<ConsoleCommandCode> command;

            IConsoleCommand<ConsoleCommandCode> ReadCommand()
            {
                string commandLine = this.Input();
                return commandLine is null ? null : new ConsoleCommand<ConsoleCommandCode>(commandLine, isCaseSensitive: false);
            }

            while (
                !((command = ReadCommand()) is null) && command.CommandCode != ConsoleCommandCode.Exit
            )
            {
                if (string.IsNullOrWhiteSpace(command.CommandLine))
                    continue;

                switch (command.CommandCode)
                {
                    case ConsoleCommandCode.Connect:
                        await this.AuthorizeAsync(command);
                        break;

                    case ConsoleCommandCode.Disconnect:
                        await this.DeauthorizeAsync();
                        break;

                    default:
                        await this.FileSystemConsoleAsync(command);
                        break;
                }
            } // while
        }

        public void Run() => this.RunAsync().Wait();

    }

}
