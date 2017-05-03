using System;
using System.Collections.Generic;
using System.Globalization;

namespace VFSCommon
{

    public class ConsoleCommand<TCommandCodeEnum> : ConsoleCommand where TCommandCodeEnum : struct
    {

        /// <summary>
        /// Command Code
        /// </summary>
        public TCommandCodeEnum? CommandCode { get; }

        private static bool IsNumeric(string s)
        {
            const NumberStyles style = NumberStyles.Integer;
            IFormatProvider provider = NumberFormatInfo.InvariantInfo;

            return
                long.TryParse(s, style, provider, out _) ||
                ulong.TryParse(s, style, provider, out _);
        }

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
            if (typeof(TCommandCodeEnum).IsEnum && !IsNumeric(this.Command))
                this.CommandCode = EnumHelper.TryParse<TCommandCodeEnum>(this.Command, ignoreCase: !this.IsCaseSensitive);
        }

        protected static ConsoleCommand<TCommandCodeEnum> CreateInternal(ConsoleCommand command) =>
            new ConsoleCommand<TCommandCodeEnum>(command.CommandLine, command.IsCaseSensitive, command.Command, command.Parameters);

        /// <summary>
        /// Parses the command from a command line
        /// </summary>
        /// <param name="commandLine">Command Line</param>
        /// <param name="isCaseSensitive">Is command a case sensitive</param>
        /// <returns>A parsed command instance</returns>
        /// <exception cref="ArgumentNullException">Throws if the command line is null</exception>
        new public static ConsoleCommand<TCommandCodeEnum> Parse(string commandLine, bool isCaseSensitive) =>
            CreateInternal(ConsoleCommand.Parse(commandLine, isCaseSensitive));

        /// <summary>
        /// Parses the command from a command line
        /// </summary>
        /// <param name="commandLine">Command Line</param>
        /// <param name="isCaseSensitive">Is command a case sensitive</param>
        /// <returns>A parsed command instance or null if the command line is null</returns>
        new public static ConsoleCommand<TCommandCodeEnum> ParseNullable(string commandLine, bool isCaseSensitive) =>
            commandLine is null ? null :
            CreateInternal(ParseInternal(commandLine, isCaseSensitive));

    }

}
