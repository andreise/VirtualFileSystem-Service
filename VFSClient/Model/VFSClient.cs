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

        private static bool EqualUserNames(string name1, string name2) => UserNameCompareRules.Comparer.Equals(name1, name2);

        private static async Task ProcessCommandHelper<TFault>(Func<Task> handler, Action<string> writeLine) where TFault : BaseFault
        {
            try
            {
                await handler();
            }
            catch (FaultException<TFault> e)
            {
                writeLine(e.Message);
            }
            catch (AggregateException e) when (e.InnerException is FaultException<TFault>)
            {
                writeLine(e.InnerException.Message);
            }
        }

        private static async Task ProcessConnectCommand(ConsoleCommand<ConsoleCommandCode> command, User user, VFSServiceClient service, Action<string> writeLine)
        {
            if (command.Parameters.Count == 0)
            {
                writeLine("User name not specified.");
                return;
            }

            string userName = command.Parameters[0];

            if (!(user.Credentials is null))
            {
                string message = Invariant($"User '{user.Credentials.UserName}' already connected.");

                if (!EqualUserNames(user.Credentials.UserName, userName))
                    message += " Please disconnect current user before connect new user.";

                writeLine(message);
                return;
            }

            await ProcessCommandHelper<ConnectFault>(
                async () =>
                {
                    var response = await service.ConnectAsync(
                        new ConnectRequest() { UserName = userName }
                    );

                    user.SetCredentials(userName, response?.Token);

                    writeLine(Invariant($"User '{response?.UserName}' connected successfully."));
                    writeLine(Invariant($"Total users: {response?.TotalUsers}."));
                },
                writeLine
            );
        }

        private static async Task ProcessDisconnectCommand(User user, VFSServiceClient service, Action<string> writeLine)
        {
            if (user.Credentials is null)
            {
                writeLine("Current user is undefined.");
                return;
            }

            await ProcessCommandHelper<DisconnectFault>(
                async () =>
                {
                    var response = await service.DisconnectAsync(
                        new DisconnectRequest() { UserName = user.Credentials.UserName, Token = user.Credentials.Token }
                    );

                    user.ResetCredentials();

                    writeLine(Invariant($"User '{response?.UserName}' disconnected."));
                },
                writeLine
            );
        }

        private static async Task ProcessFSCommand(ConsoleCommand<ConsoleCommandCode> command, User user, VFSServiceClient service, Action<string> writeLine)
        {
            if (user.Credentials is null)
            {
                writeLine("Please connect to the host before sending to it any other commands.");
                return;
            }

            await ProcessCommandHelper<FSCommandFault>(
                async () =>
                {
                    var response = await service.FSCommandAsync(
                        new FSCommandRequest() { UserName = user.Credentials.UserName, Token = user.Credentials.Token, CommandLine = command.CommandLine }
                    );

                    writeLine(response?.ResponseMessage);
                },
                writeLine
            );
        }

        private static void HandleCallback(FileSystemChangedData data, User user, Action<string> writeLine)
        {
            if (data is null)
                return;

            if (user.Credentials is null)
                return;

            if (EqualUserNames(data.UserName, user.Credentials.UserName))
                return;

            writeLine(Invariant($"User '{data.UserName}' performs command: {data.CommandLine}"));
        }

        public static async Task Run(Func<string> readLine, Action<string> writeLine)
        {
            if (readLine is null)
                throw new ArgumentNullException(nameof(readLine));

            if (writeLine is null)
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
                !((command = readCommand()) is null) && command.CommandCode != ConsoleCommandCode.Exit
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
