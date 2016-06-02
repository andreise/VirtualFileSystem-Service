using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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

        static ConsoleCommand ParseCommandLine(string commandLine) =>
            string.IsNullOrWhiteSpace(commandLine) ? null : ConsoleCommand.Parse(commandLine);

        static void WriteCommandExecutionTimeoutExpired(ConsoleCommand command) =>
            Console.WriteLine(Invariant($"Command execution timeout expired (command: {command.CommandCode})."));

        static void Run()
        {
            Console.WriteLine("Virtual File System Client");
            Console.WriteLine("Put command to work with the file system, or type Exit to exit");

            string defaultEndpointAuthority = ConfigurationManager.AppSettings["DefaultEndpointAuthority"];

            TimeSpan taskTimeout = TimeSpan.FromTicks(
                TimeSpan.TicksPerMillisecond *
                XmlConvert.ToInt32(ConfigurationManager.AppSettings["TaskTimeoutMilliseconds"])
            );

            string commandLine;
            ConsoleCommand command;
            VFSServiceClient service = new VFSServiceClient(new InstanceContext(new VFSServiceCallbackHandler()));
            UserInfo = null;

            while (
                (
                    (object)(commandLine = Console.ReadLine()) != null
                ) &&
                (
                    (object)(command = ParseCommandLine(commandLine)) == null ||
                    command.CommandCode != ConsoleCommandCode.Exit
                )
            )
            {
                switch (command.CommandCode)
                {
                    case ConsoleCommandCode.Connect:
                        {
                            if (command.Parameters.Count == 0)
                                Console.WriteLine("Server address and user name are not specified.");
                            else if (command.Parameters.Count == 1)
                                Console.WriteLine("User name is not specified.");
                            else
                            {
                                if (
                                    new Uri(
                                        service.Endpoint.ListenUri.ToString().Replace(service.Endpoint.ListenUri.Authority, command.Parameters[0])
                                    ).ToString() != service.Endpoint.ListenUri.ToString()
                                )
                                    service.Endpoint.ListenUri = new Uri(
                                        service.Endpoint.Address.Uri.ToString().Replace(defaultEndpointAuthority, command.Parameters[0])
                                    );

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

                    case 0:
                        {
                            if ((object)UserInfo == null)
                            {
                                Console.WriteLine("Current user is undefined.");
                                break;
                            }

                            try
                            {
                                FSCommandResponse response = service.FSCommand(
                                    new FSCommandRequest() { UserName = UserInfo.UserName, Token = UserInfo.Token, CommandLine = commandLine }
                                );
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
