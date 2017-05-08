using System;
using System.Threading.Tasks;
using VirtualFileSystem.Common;
using static System.FormattableString;

namespace VirtualFileSystemClient.Model
{

    internal sealed class VFSClient : VFSClientBase
    {

        private readonly Func<string> Input;

        public VFSClient(Func<string> input, Action<string> output) : base(output)
        {
            if (input is null)
                throw new ArgumentNullException(nameof(input));

            this.Input = input;
        }

        private bool alreadyRun;

        private readonly object runLock = new object();

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
                            await this.ProcessConnectCommand(command);
                            break;

                        case ConsoleCommandCode.Disconnect:
                            await this.ProcessDisconnectCommand();
                            break;

                        default:
                            await this.ProcessVFSCommand(command);
                            break;
                    }
                } // while
            }
            finally
            {
                this.Service.Abort();
            }
        }

    }

}
