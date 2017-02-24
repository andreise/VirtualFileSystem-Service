using System;
using System.Configuration;
using System.ServiceModel;
using System.Xml;
using VFSCommon;
using static System.FormattableString;

namespace VFSClient
{
    using Model;
    using VFSServiceReference;

    sealed class UserInfo
    {
        public string UserName { get; }

        public byte[] Token { get; }

        public UserInfo(string userName, byte[] token)
        {
            this.UserName = userName;
            this.Token = token;
        }
    }

    sealed class VFSServiceCallbackHandler : IVFSServiceCallback
    {
        public void FileSystemChangedNotify(FileSystemChangedData data)
        {
            if (data.UserName == Program.UserInfo.UserName)
                return;

            Console.WriteLine(Invariant($"User '{data.UserName}' performs command: {data.CommandLine}."));
        }
    }

    static class Program
    {

        public static UserInfo UserInfo { get; private set; }

        static void WriteCommandExecutionTimeoutExpired(ConsoleCommand<ConsoleCommandCode> command) =>
            Console.WriteLine(Invariant($"Command execution timeout expired (command: {command.Command})."));

        static void Run()
        {
            Console.WriteLine("Virtual File System Client");
            Console.WriteLine(Invariant($"Connect to host and send commands to the file system, or type {nameof(ConsoleCommandCode.Quit)} to exit"));

            string defaultEndpointAuthority = ConfigurationManager.AppSettings["DefaultEndpointAuthority"];

            TimeSpan taskTimeout = TimeSpan.FromTicks(
                TimeSpan.TicksPerMillisecond *
                XmlConvert.ToInt32(ConfigurationManager.AppSettings["TaskTimeoutMilliseconds"])
            );

            VFSServiceClient service = new VFSServiceClient(new InstanceContext(new VFSServiceCallbackHandler()));
            UserInfo = null;

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
                                if (
                                    new Uri(
                                        service.Endpoint.ListenUri.ToString().Replace(service.Endpoint.ListenUri.Authority, command.Parameters[0])
                                    ).ToString() != service.Endpoint.ListenUri.ToString()
                                )
                                {
                                    service.Endpoint.ListenUri = new Uri(
                                        service.Endpoint.Address.Uri.ToString().Replace(defaultEndpointAuthority, command.Parameters[0])
                                    );
                                }

                                try
                                {
                                    ConnectResponse response = service.Connect(new ConnectRequest() { UserName = command.Parameters[1] });
                                    UserInfo = new UserInfo(command.Parameters[1], response.Token);
                                    Console.WriteLine(Invariant($"User '{response.UserName}' connected successfully."));
                                    Console.WriteLine(Invariant($"Total users: {response.TotalUsers}."));
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
                                if (
                                    !service.DisconnectAsync(new DisconnectRequest() { UserName = UserInfo.UserName, Token = UserInfo.Token }).Wait(taskTimeout)
                                )
                                    WriteCommandExecutionTimeoutExpired(command);
                                else
                                    Console.WriteLine(Invariant($"User '{UserInfo.UserName}' disconnected."));

                                UserInfo = null;
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
                                FSCommandResponse response = service.FSCommand(
                                    new FSCommandRequest() { UserName = UserInfo.UserName, Token = UserInfo.Token, CommandLine = command.CommandLine }
                                );
                                Console.WriteLine(response.ResponseMessage);
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

        static void Main(string[] args)
        {
            Run();
        }

    }
}
