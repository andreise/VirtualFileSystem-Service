using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using VirtualFileSystem.Common;
using VirtualFileSystemClient.VFSServiceReference;
using static System.FormattableString;

namespace VirtualFileSystemClient.Model
{

    internal sealed class VFSClient : VFSClientBase
    {

        private readonly Func<string> Input;

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

        protected override VFSServiceClient Service { get; }

        public VFSClient(Func<string> input, Action<string> output) : base(output)
        {
            if (input is null)
                throw new ArgumentNullException(nameof(input));

            this.Input = input;

            this.Service = new VFSServiceClient(
                new InstanceContext(new VFSServiceCallbackHandler(this.HandleCallback))
            );
        }

        public async Task Run()
        {
            this.Output("Virtual File System Client");
            this.Output(Invariant($"Connect to host specified in the endpoint and send commands to the file system, or type '{nameof(ConsoleCommandCode.Quit)}' or '{nameof(ConsoleCommandCode.Exit)}' to exit."));
            this.Output(Invariant($"Type '{ConsoleCommandCode.Connect} UserName'..."));

            IConsoleCommand<ConsoleCommandCode> command;

            IConsoleCommand<ConsoleCommandCode> ReadCommand() => ConsoleCommandParser.TryParse<ConsoleCommandCode>(this.Input(), isCaseSensitive: false);

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

    }

}
