using Common.Enums;
using System;
using System.Globalization;

namespace VirtualFileSystem.Common.Console
{

    public class ConsoleCommand<TCommandCodeEnum> : ConsoleCommand, IConsoleCommand<TCommandCodeEnum> where TCommandCodeEnum : struct
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

        private TCommandCodeEnum? GetCommandCode()
        {
            var commandCode = EnumHelper.TryParse<TCommandCodeEnum>(this.Command, ignoreCase: !this.IsCaseSensitive);
            return commandCode is null || IsNumeric(this.Command) ? null : commandCode;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="commandLine">Command Line</param>
        /// <param name="isCaseSensitive">Is case sensitive command line</param>
        /// <exception cref="ArgumentNullException">Throws if command line is null</exception>
        public ConsoleCommand(string commandLine, bool isCaseSensitive) : base(commandLine, isCaseSensitive)
        {
            this.CommandCode = this.GetCommandCode();
        }

    }

}
