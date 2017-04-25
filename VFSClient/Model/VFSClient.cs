using System;
using System.ServiceModel;
using System.Threading.Tasks;
using VFSCommon;
using static System.FormattableString;

namespace VFSClient.Model
{
    using Security;
    using VFSServiceReference;

    internal sealed class VFSClient
    {

        private readonly Func<string> readLine;

        private readonly Action<string> writeLine;

        public VFSClient(Func<string> readLine, Action<string> writeLine)
        {
            if ((object)readLine == null)
                throw new ArgumentNullException(nameof(readLine));

            if ((object)writeLine == null)
                throw new ArgumentNullException(nameof(writeLine));

            this.readLine = readLine;
            this.writeLine = writeLine;
        }

        private async Task ProcessConnectCommand(VFSServiceClient service, User user, ConsoleCommand<ConsoleCommandCode> command)
        {
            if (command.Parameters.Count == 0)
            {
                this.writeLine("User name not specified.");
                return;
            }

            string userName = command.Parameters[0];

            if ((object)user.Credentials != null)
            {
                string message = Invariant($"User '{user.Credentials.UserName}' already connected.");

                if (!string.Equals(user.Credentials.UserName, userName, StringComparison.InvariantCultureIgnoreCase))
                    message += " Please disconnect current user before connect new user.";

                this.writeLine(message);
                return;
            }

            try
            {
                var response = await service.ConnectAsync(
                    new ConnectRequest() { UserName = userName }
                );

                user.Credentials = new UserCredentials(userName, response?.Token);

                this.writeLine(Invariant($"User '{response?.UserName}' connected successfully."));
                this.writeLine(Invariant($"Total users: {response?.TotalUsers}."));
            }
            catch (FaultException<ConnectFault> e)
            {
                this.writeLine(e.Message);
            }
        }

        private async Task ProcessDisconnectCommand(VFSServiceClient service, User user)
        {
            if ((object)user.Credentials == null)
            {
                this.writeLine("Current user is undefined.");
                return;
            }

            try
            {
                var response = await service.DisconnectAsync(
                    new DisconnectRequest() { UserName = user.Credentials.UserName, Token = user.Credentials.Token }
                );

                user.Credentials = null;

                this.writeLine(Invariant($"User '{response?.UserName}' disconnected."));
            }
            catch (FaultException<DisconnectFault> e)
            {
                this.writeLine(e.Message);
            }
            catch (AggregateException e) when (e.InnerException is FaultException<DisconnectFault>)
            {
                this.writeLine(e.InnerException.Message);
            }
        }

        private async Task ProcessFSCommand(VFSServiceClient service, User user, ConsoleCommand<ConsoleCommandCode> command)
        {
            if ((object)user.Credentials == null)
            {
                this.writeLine("Please connect to the host before sending to it any other commands.");
                return;
            }

            try
            {
                var response = await service.FSCommandAsync(
                    new FSCommandRequest() { UserName = user.Credentials.UserName, Token = user.Credentials.Token, CommandLine = command.CommandLine }
                );

                this.writeLine(response?.ResponseMessage);
            }
            catch (FaultException<FSCommandFault> e)
            {
                this.writeLine(e.Message);
            }
            catch (AggregateException e) when (e.InnerException is FaultException<FSCommandFault>)
            {
                this.writeLine(e.InnerException.Message);
            }
        }

        public async Task Run()
        {
            this.writeLine("Virtual File System Client");
            this.writeLine(Invariant($"Connect to host specified in the endpoint and send commands to the file system, or type '{nameof(ConsoleCommandCode.Quit)}' or '{nameof(ConsoleCommandCode.Exit)}' to exit."));
            this.writeLine(Invariant($"Type '{ConsoleCommandCode.Connect} UserName'..."));

            var user = new User();

            VFSServiceClient service = new VFSServiceClient(
                new InstanceContext(
                    new VFSServiceCallbackHandler(
                        data =>
                        {
                            if ((object)data == null)
                                return;

                            if ((object)user.Credentials == null)
                                return;

                            if (data.UserName == user.Credentials.UserName)
                                return;

                            this.writeLine(Invariant($"User '{data.UserName}' performs command: {data.CommandLine}"));
                        }
                    )
                )
            );

            ConsoleCommand<ConsoleCommandCode> command;
            while (
                (object)(command = ConsoleCommand<ConsoleCommandCode>.ParseNullable(this.readLine(), isCaseSensitive: false)) != null &&
                command.CommandCode != ConsoleCommandCode.Exit
            )
            {
                if (string.IsNullOrWhiteSpace(command.CommandLine))
                    continue;

                switch (command.CommandCode)
                {
                    case ConsoleCommandCode.Connect:
                        await this.ProcessConnectCommand(service, user, command);
                        break;

                    case ConsoleCommandCode.Disconnect:
                        await this.ProcessDisconnectCommand(service, user);
                        break;

                    default:
                        await this.ProcessFSCommand(service, user, command);
                        break;
                }
            } // while

        } // Run method

    }

}
