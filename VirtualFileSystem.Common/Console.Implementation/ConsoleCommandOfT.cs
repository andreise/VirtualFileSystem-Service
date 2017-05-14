using Common.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace VirtualFileSystem.Common.Console.Implementation
{

    internal sealed class ConsoleCommand<TCommandCodeEnum> : ConsoleCommand, IConsoleCommand<TCommandCodeEnum> where TCommandCodeEnum : struct
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
        public ConsoleCommand(string commandLine, bool isCaseSensitive, string command, IReadOnlyList<string> parameters = null)
            : base(commandLine, isCaseSensitive, command, parameters)
        {
            var commandCode = EnumHelper.TryParse<TCommandCodeEnum>(this.Command, ignoreCase: !this.IsCaseSensitive);
            if (!(commandCode is null) && !IsNumeric(this.Command))
                this.CommandCode = commandCode;
        }

    }

}
