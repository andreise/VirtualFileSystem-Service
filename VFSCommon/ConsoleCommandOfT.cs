using System.Collections.Generic;

namespace VFSCommon
{

    public class ConsoleCommand<TCommandCodeEnum> : ConsoleCommand where TCommandCodeEnum : struct
    {

        /// <summary>
        /// Command Code
        /// </summary>
        public TCommandCodeEnum? CommandCode { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="commandLine">Command Line</param>
        /// <param name="isCaseSensitive">Is command a case sensitive</param>
        /// <param name="command">Command</param>
        /// <param name="parameters">Command Parameters</param>
        protected ConsoleCommand(string commandLine, bool isCaseSensitive, string command, IEnumerable<string> parameters = null)
            : base(commandLine, isCaseSensitive, command, parameters)
        {
            this.CommandCode = EnumHelper.TryParse<TCommandCodeEnum>(this.Command, ignoreCase: !this.IsCaseSensitive);
        }

        /// <summary>
        /// Parses the command from a command line
        /// </summary>
        /// <param name="commandLine">Command Line</param>
        /// <param name="isCaseSensitive">Is command a case sensitive</param>
        /// <returns>Parsed command instance</returns>
        /// <exception cref="ArgumentNullException">Throws if the command line is null</exception>
        new public static ConsoleCommand<TCommandCodeEnum> Parse(string commandLine, bool isCaseSensitive = false)
        {
            var command = ConsoleCommand.Parse(commandLine);
            return new ConsoleCommand<TCommandCodeEnum>(command.CommandLine, command.IsCaseSensitive, command.Command, command.Parameters);
        }

    }

}
