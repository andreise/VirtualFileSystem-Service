using System;
using System.ServiceModel;
using System.Threading.Tasks;
using VFSCommon;
using static System.FormattableString;

namespace VFSClient
{
    using Model;
    using VFSServiceReference;

    internal sealed class VFSClientModel
    {
        public UserInfo UserInfo { get; private set; }

        public async Task Run()
        {
            Console.WriteLine("Virtual File System Client");
            Console.WriteLine(Invariant($"Connect to host and send commands to the file system, or type {nameof(ConsoleCommandCode.Quit)} to exit"));

            UserInfo = null;

            VFSServiceClient service = new VFSServiceClient(new InstanceContext(new VFSServiceCallbackHandler(this)));

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
                                Console.WriteLine("Server address and user name are not specified.");
                            }
                            else if (command.Parameters.Count == 1)
                            {
                                Console.WriteLine("User name is not specified.");
                            }
                            else
                            {
                                try
                                {
                                    var response = await service.ConnectAsync(
                                        new ConnectRequest() { UserName = command.Parameters[1] }
                                    );

                                    UserInfo = new UserInfo(command.Parameters[1], response?.Token);

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
