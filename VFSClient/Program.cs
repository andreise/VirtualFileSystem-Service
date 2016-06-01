using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace VFSClient
{
    using Model;
    using VFSServiceReference;

    static class Program
    {

        static ConsoleCommand ParseCommandLine(string commandLine) =>
            string.IsNullOrWhiteSpace(commandLine) ? null : ConsoleCommand.Parse(commandLine);

        static void Main(string[] args)
        {

            Console.WriteLine("Virtual File System Client");

            Console.WriteLine("Put command to work with the file system, ot type Exit to exit");


            using (VFSServiceClient service = new VFSServiceClient())
            {
                service.Endpoint.ListenUri = new Uri("http://localhost1:8000/VFSService/service");
                service.Endpoint.ListenUri = new Uri("http://localhost:8000/VFSService/service");
                string commandLine;
                ConsoleCommand command;
                while (
                    (object)(commandLine = Console.ReadLine()) != null &&
                    (
                        (object)(command = ParseCommandLine(commandLine)) == null || !command.IsCommand(ConsoleCommandCodes.Exit)
                    )
                )
                {
                    int i = service.Connect(new ConnectRequest() { UserName = "admin" }).ConnectedUsers;
                }
            }

        }
    }
}
