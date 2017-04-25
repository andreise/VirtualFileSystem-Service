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

        private static async Task ProcessConnectCommand(Action<string> writeLine, VFSServiceClient service, User user, ConsoleCommand<ConsoleCommandCode> command)
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

                if (!string.Equals(user.Credentials.UserName, userName, StringComparison.InvariantCultureIgnoreCase))
                    message += " Please disconnect current user before connect new user.";

                writeLine(message);
                return;
            }

            try
            {
                var response = await service.ConnectAsync(
                    new ConnectRequest() { UserName = userName }
                );

                user.Credentials = new UserCredentials(userName, response?.Token);

                writeLine(Invariant($"User '{response?.UserName}' connected successfully."));
                writeLine(Invariant($"Total users: {response?.TotalUsers}."));
            }
            catch (FaultException<ConnectFault> e)
            {
                writeLine(e.Message);
            }
        }

        private static async Task ProcessDisconnectCommand(Action<string> writeLine, VFSServiceClient service, User user)
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

                user.Credentials = null;

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

        private static async Task ProcessFSCommand(Action<string> writeLine, VFSServiceClient service, User user, ConsoleCommand<ConsoleCommandCode> command)
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

                            writeLine(Invariant($"User '{data.UserName}' performs command: {data.CommandLine}"));
                        }
                    )
                )
            );

            ConsoleCommand<ConsoleCommandCode> command;
            while (
                (object)(command = ConsoleCommand<ConsoleCommandCode>.ParseNullable(readLine(), isCaseSensitive: false)) != null &&
                command.CommandCode != ConsoleCommandCode.Exit
            )
            {
                if (string.IsNullOrWhiteSpace(command.CommandLine))
                    continue;

                switch (command.CommandCode)
                {
                    case ConsoleCommandCode.Connect:
                        await ProcessConnectCommand(writeLine, service, user, command);
                        break;

                    case ConsoleCommandCode.Disconnect:
                        await ProcessDisconnectCommand(writeLine, service, user);
                        break;

                    default:
                        await ProcessFSCommand(writeLine, service, user, command);
                        break;
                }
            } // while

        } // Run method

    }

}
