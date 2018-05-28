using Common.Enums;
using System;
using System.Globalization;
using static System.FormattableString;

namespace VirtualFileSystem.Common.Console
{

    public class ConsoleCommand<TCommandCodeEnum> : ConsoleCommand, IConsoleCommand<TCommandCodeEnum>
        where TCommandCodeEnum : struct, Enum
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
            if (IsNumeric(this.Command))
                return null;

            return EnumHelper.TryParse<TCommandCodeEnum>(this.Command, ignoreCase: !this.IsCaseSensitive);
        }

        private static void ValidateCommandCodeEnum()
        {
            if (!EnumHelper.IsEnum<TCommandCodeEnum>())
                throw new ArgumentException(Invariant($"{nameof(TCommandCodeEnum)} is not an enumeration type."));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="commandLine">Command Line</param>
        /// <param name="isCaseSensitive">Is case sensitive command line</param>
        /// <exception cref="ArgumentNullException">Throws if command line is null</exception>
        /// <exception cref="ArgumentException">Throws if TCommandCodeEnum is not an enumeration type</exception>
        public ConsoleCommand(string commandLine, bool isCaseSensitive) : base(commandLine, isCaseSensitive)
        {
            ValidateCommandCodeEnum();

            this.CommandCode = this.GetCommandCode();
        }

    }

}
