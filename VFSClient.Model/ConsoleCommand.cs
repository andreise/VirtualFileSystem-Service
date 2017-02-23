using System;
using System.Linq;
using VFSCommon;

namespace VFSClient.Model
{

    /// <summary>
    /// Console Command
    /// </summary>
    public class ConsoleCommand : BaseConsoleCommand
    {

        /// <summary>
        /// Command Code
        /// </summary>
        public ConsoleCommandCode CommandCode { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="parameters">Command Parameters</param>
        /// <exception cref="ArgumentNullException">Trows if the command is null</exception>
        /// <exception cref="ArgumentException">Trows if the command is empty or any command parameter is null or empty</exception>
        public ConsoleCommand(string command, string[] parameters = null) : base(command, parameters)
        {
            ConsoleCommandCode commandCode;
            if (Enum.TryParse(value: this.Command, ignoreCase: true, result: out commandCode))
                this.CommandCode = commandCode;
        }

        /// <summary>
        /// Parses the command from a command line
        /// </summary>
        /// <param name="commandLine">Client command from a command line</param>
        /// <returns>A client command instance</returns>
        /// <exception cref="ArgumentNullException">Throws if the command is null</exception>
        /// <exception cref="ArgumentException">Throws if the command is empty</exception>
        new public static ConsoleCommand Parse(string commandLine)
        {
            BaseConsoleCommand command = BaseConsoleCommand.Parse(commandLine);
            return new ConsoleCommand(command.Command, command.Parameters.ToArray());
        }

    }

}
