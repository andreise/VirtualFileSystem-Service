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
        public UserInfo UserInfo { get; private set; }

        public async Task Run()
        {
            Console.WriteLine("Virtual File System Client");
            Console.WriteLine(Invariant($"Connect to host specified in the endpoint and send commands to the file system, or type '{nameof(ConsoleCommandCode.Quit)}' or '{nameof(ConsoleCommandCode.Exit)}' to exit."));
            Console.WriteLine(Invariant($"Type '{ConsoleCommandCode.Connect} UserName'..."));

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

                            Console.WriteLine(Invariant($"User '{data.UserName}' performs command: {data.CommandLine}"));
                        }
                    )
                )
            );

            ConsoleCommand<ConsoleCommandCode> command;
            while (
                (object)(command = ConsoleCommand<ConsoleCommandCode>.ParseNullable(Console.ReadLine(), isCaseSensitive: false)) != null &&
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
                                Console.WriteLine("User name not specified.");
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

                                    Console.WriteLine(Invariant($"User '{response?.UserName}' connected successfully."));
                                    Console.WriteLine(Invariant($"Total users: {response?.TotalUsers}."));
                                }
                                catch (FaultException<ConnectFault> e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                            }
                        }
                        break;

                    case ConsoleCommandCode.Disconnect:
                        {
                            if ((object)UserInfo == null)
                            {
                                Console.WriteLine("Current user is undefined.");
                                break;
                            }

                            try
                            {
                                var response = await service.DisconnectAsync(
                                    new DisconnectRequest() { UserName = UserInfo.UserName, Token = UserInfo.Token }
                                );

                                UserInfo = null;

                                Console.WriteLine(Invariant($"User '{response?.UserName}' disconnected."));
                            }
                            catch (FaultException<DisconnectFault> e)
                            {
                                Console.WriteLine(e.Message);
                            }
                            catch (AggregateException e) when (e.InnerException is FaultException<DisconnectFault>)
                            {
                                Console.WriteLine(e.InnerException.Message);
                            }
                        }
                        break;

                    default:
                        {
                            if ((object)UserInfo == null)
                            {
                                Console.WriteLine("Please connect to the host before sending to it any other commands.");
                                break;
                            }

                            try
                            {
                                var response = await service.FSCommandAsync(
                                    new FSCommandRequest() { UserName = UserInfo.UserName, Token = UserInfo.Token, CommandLine = command.CommandLine }
                                );

                                Console.WriteLine(response?.ResponseMessage);
                            }
                            catch (FaultException<FSCommandFault> e)
                            {
                                Console.WriteLine(e.Message);
                            }
                            catch (AggregateException e) when (e.InnerException is FaultException<FSCommandFault>)
                            {
                                Console.WriteLine(e.InnerException.Message);
                            }
                        }
                        break;
                }
            } // while

        } // Run method

    }

}
