using System;
using System.Threading.Tasks;
using VirtualFileSystem.Common;
using static System.FormattableString;

namespace VirtualFileSystemClient.Model.Common
{
    using Security;

    public abstract class VFSClient<TConnectException, TDisconnectException, TVFSException>
        where TConnectException : Exception
        where TDisconnectException : Exception
        where TVFSException : Exception
    {

        protected static bool EqualUserNames(string name1, string name2) => UserNameComparerProvider.Default.Equals(name1, name2);

        protected readonly User User = new User();

        protected readonly Action<string> Output;

        public VFSClient(Action<string> output)
        {
            if (output is null)
                throw new ArgumentNullException(nameof(output));

            this.Output = output;
        }

        protected abstract Task ConnectCommandHandler(string userName);

        protected abstract Task DisconnectCommandHandler();

        protected abstract Task VFSCommandHandler();

        protected async Task ProcessCommand<TException>(Func<Task> handler) where TException : Exception
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

        protected async Task ProcessConnectCommand(IConsoleCommand<ConsoleCommandCode> command)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));

            if (command.Parameters.Count == 0)
            {
                this.Output("User name not specified.");
                return;
            }

            string userName = command.Parameters[0];

            if (!(this.User.Credentials is null))
            {
                if (!EqualUserNames(this.User.Credentials.UserName, userName))
                {
                    this.Output(Invariant($"Please disconnect current user ('{this.User.Credentials.UserName}') before connect new user."));
                    return;
                }
            }

            await this.ProcessCommand<TConnectException>(async () => await this.ConnectCommandHandler(userName));
        }


        protected async Task ProcessDisconnectCommand()
        {
            if (this.User.Credentials is null)
            {
                this.Output("Current user is undefined.");
                return;
            }

            await this.ProcessCommand<TDisconnectException>(this.DisconnectCommandHandler);
        }

        protected async Task ProcessVFSCommand()
        {
            if (this.User.Credentials is null)
            {
                this.Output("Please connect to the host before sending to it any other commands.");
                return;
            }

            await this.ProcessCommand<TVFSException>(this.VFSCommandHandler);
        }

    }

}
