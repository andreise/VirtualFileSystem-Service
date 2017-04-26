using System;
using System.ServiceModel;
using System.Threading.Tasks;
using VFSCommon;
using static System.FormattableString;

namespace VFSClient.Model
{
    using Security;
    using VFSServiceReference;

    internal static class VFSClient
    {

        private static bool EqualUserNames(string name1, string name2) => string.Equals(name1, name2, StringComparison.InvariantCultureIgnoreCase);

        private static async Task ProcessConnectCommand(ConsoleCommand<ConsoleCommandCode> command, User user, VFSServiceClient service, Action<string> writeLine)
        {
            if (command.Parameters.Count == 0)
            {
                writeLine("User name not specified.");
                return;
            }

            string userName = command.Parameters[0];

            if ((object)user.Credentials != null)
            {
                string message = Invariant($"User '{user.Credentials.UserName}' already connected.");

                if (!EqualUserNames(user.Credentials.UserName, userName))
                    message += " Please disconnect current user before connect new user.";

                writeLine(message);
                return;
            }

            try
            {
                var response = await service.ConnectAsync(
                    new ConnectRequest() { UserName = userName }
                );

                user.SetCredentials(userName, response?.Token);

                writeLine(Invariant($"User '{response?.UserName}' connected successfully."));
                writeLine(Invariant($"Total users: {response?.TotalUsers}."));
            }
            catch (FaultException<ConnectFault> e)
            {
                writeLine(e.Message);
            }
        }

        private static async Task ProcessDisconnectCommand(User user, VFSServiceClient service, Action<string> writeLine)
        {
            if ((object)user.Credentials == null)
            {
                writeLine("Current user is undefined.");
                return;
            }

            try
            {
                var response = await service.DisconnectAsync(
                    new DisconnectRequest() { UserName = user.Credentials.UserName, Token = user.Credentials.Token }
                );

                user.ResetCredentials();

                writeLine(Invariant($"User '{response?.UserName}' disconnected."));
            }
            catch (FaultException<DisconnectFault> e)
            {
                writeLine(e.Message);
            }
            catch (AggregateException e) when (e.InnerException is FaultException<DisconnectFault>)
            {
                writeLine(e.InnerException.Message);
            }
        }

        private static async Task ProcessFSCommand(ConsoleCommand<ConsoleCommandCode> command, User user, VFSServiceClient service, Action<string> writeLine)
        {
            if ((object)user.Credentials == null)
            {
                writeLine("Please connect to the host before sending to it any other commands.");
                return;
            }

            try
            {
                var response = await service.FSCommandAsync(
                    new FSCommandRequest() { UserName = user.Credentials.UserName, Token = user.Credentials.Token, CommandLine = command.CommandLine }
                );

                writeLine(response?.ResponseMessage);
            }
            catch (FaultException<FSCommandFault> e)
            {
                writeLine(e.Message);
            }
            catch (AggregateException e) when (e.InnerException is FaultException<FSCommandFault>)
            {
                writeLine(e.InnerException.Message);
            }
        }

        private static void HandleCallback(FileSystemChangedData data, User user, Action<string> writeLine)
        {
            if ((object)data == null)
                return;

            if ((object)user.Credentials == null)
                return;

            if (EqualUserNames(data.UserName, user.Credentials.UserName))
                return;

            writeLine(Invariant($"User '{data.UserName}' performs command: {data.CommandLine}"));
        }

        public static async Task Run(Func<string> readLine, Action<string> writeLine)
        {
            if ((object)readLine == null)
                throw new ArgumentNullException(nameof(readLine));

            if ((object)writeLine == null)
                throw new ArgumentNullException(nameof(writeLine));

            writeLine("Virtual File System Client");
            writeLine(Invariant($"Connect to host specified in the endpoint and send commands to the file system, or type '{nameof(ConsoleCommandCode.Quit)}' or '{nameof(ConsoleCommandCode.Exit)}' to exit."));
            writeLine(Invariant($"Type '{ConsoleCommandCode.Connect} UserName'..."));

            var user = new User();

            var service = new VFSServiceClient(
                new InstanceContext(
                    new VFSServiceCallbackHandler(data => HandleCallback(data, user, writeLine))
                )
            );

            Func<ConsoleCommand<ConsoleCommandCode>> readCommand = () => ConsoleCommand<ConsoleCommandCode>.ParseNullable(readLine(), isCaseSensitive: false);

            ConsoleCommand<ConsoleCommandCode> command;

            while (
                (object)(command = readCommand()) != null &&
                command.CommandCode != ConsoleCommandCode.Exit
            )
            {
                if (string.IsNullOrWhiteSpace(command.CommandLine))
                    continue;

                switch (command.CommandCode)
                {
                    case ConsoleCommandCode.Connect:
                        await ProcessConnectCommand(command, user, service, writeLine);
                        break;

                    case ConsoleCommandCode.Disconnect:
                        await ProcessDisconnectCommand(user, service, writeLine);
                        break;

                    default:
                        await ProcessFSCommand(command, user, service, writeLine);
                        break;
                }
            } // while

        } // Run

    }

}
