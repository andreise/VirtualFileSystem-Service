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

        public UserInfo UserInfo { get; private set; }

        public async Task Run()
        {
            this.writeLine("Virtual File System Client");
            this.writeLine(Invariant($"Connect to host specified in the endpoint and send commands to the file system, or type '{nameof(ConsoleCommandCode.Quit)}' or '{nameof(ConsoleCommandCode.Exit)}' to exit."));
            this.writeLine(Invariant($"Type '{ConsoleCommandCode.Connect} UserName'..."));

            UserInfo = null;

            VFSServiceClient service = new VFSServiceClient(
                new InstanceContext(
                    new VFSServiceCallbackHandler(
                        data =>
                        {
                            if ((object)data == null)
                                return;

                            if ((object)this.UserInfo == null)
                                return;

                            if (data.UserName == this.UserInfo.UserName)
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
                        {
                            if (command.Parameters.Count == 0)
                            {
                                this.writeLine("User name not specified.");
                            }
                            else
                            {
                                string userName = command.Parameters[0];
                                try
                                {
                                    var response = await service.ConnectAsync(
                                        new ConnectRequest() { UserName = userName }
                                    );

                                    UserInfo = new UserInfo(userName, response?.Token);

                                    this.writeLine(Invariant($"User '{response?.UserName}' connected successfully."));
                                    this.writeLine(Invariant($"Total users: {response?.TotalUsers}."));
                                }
                                catch (FaultException<ConnectFault> e)
                                {
                                    this.writeLine(e.Message);
                                }
                            }
                        }
                        break;

                    case ConsoleCommandCode.Disconnect:
                        {
                            if ((object)UserInfo == null)
                            {
                                this.writeLine("Current user is undefined.");
                                break;
                            }

                            try
                            {
                                var response = await service.DisconnectAsync(
                                    new DisconnectRequest() { UserName = UserInfo.UserName, Token = UserInfo.Token }
                                );

                                UserInfo = null;

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
                        break;

                    default:
                        {
                            if ((object)UserInfo == null)
                            {
                                this.writeLine("Please connect to the host before sending to it any other commands.");
                                break;
                            }

                            try
                            {
                                var response = await service.FSCommandAsync(
                                    new FSCommandRequest() { UserName = UserInfo.UserName, Token = UserInfo.Token, CommandLine = command.CommandLine }
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
                        break;
                }
            } // while

        } // Run method

    }

}
