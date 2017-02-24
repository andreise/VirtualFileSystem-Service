using System.Linq;

namespace VFSCommon
{

    public class BaseConsoleCommand<TCommandCodeEnum> : BaseConsoleCommand where TCommandCodeEnum : struct
    {

        /// <summary>
        /// Command Code
        /// </summary>
        public TCommandCodeEnum? CommandCode { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="parameters">Command Parameters</param>
        /// <exception cref="ArgumentNullException">Throws if the command is null</exception>
        /// <exception cref="ArgumentException">Throws if the command is empty or any command parameter is null or empty</exception>
        public BaseConsoleCommand(string command, string[] parameters = null) : base(command, parameters)
        {
            this.CommandCode = EnumHelper.TryParse<TCommandCodeEnum>(this.Command, ignoreCase: true);
        }

        /// <summary>
        /// Parses the command from a command line
        /// </summary>
        /// <param name="commandLine">Client command from a command line</param>
        /// <returns>A client command instance</returns>
        /// <exception cref="ArgumentNullException">Throws if the command is null</exception>
        /// <exception cref="ArgumentException">Throws if the command is empty</exception>
        new public static BaseConsoleCommand<TCommandCodeEnum> Parse(string commandLine)
        {
            var command = BaseConsoleCommand.Parse(commandLine);
            return new BaseConsoleCommand<TCommandCodeEnum>(command.Command, command.Parameters.ToArray());
        }

    }

}
