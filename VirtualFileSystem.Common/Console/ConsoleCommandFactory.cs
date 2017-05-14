using System;
using System.Linq;

namespace VirtualFileSystem.Common.Console
{
    using Implementation;

    public static class ConsoleCommandFactory
    {

        private static IConsoleCommand ParseInternal(string commandLine, bool isCaseSensitive)
        {
            var commandLineItems = commandLine.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            return new ConsoleCommand(
                commandLine,
                isCaseSensitive,
                commandLineItems.FirstOrDefault() ?? string.Empty,
                commandLineItems.Skip(1).ToArray()
            );
        }

        public static IConsoleCommand Parse(string commandLine, bool isCaseSensitive)
        {
            if (commandLine is null)
                throw new ArgumentNullException(nameof(commandLine));

            return ParseInternal(commandLine, isCaseSensitive);
        }

        public static IConsoleCommand TryParse(string commandLine, bool isCaseSensitive)
        {
            if (commandLine is null)
                return null;

            return ParseInternal(commandLine, isCaseSensitive);
        }

        private static IConsoleCommand<TCommandCodeEnum> ParseInternal<TCommandCodeEnum>(string commandLine, bool isCaseSensitive) where TCommandCodeEnum : struct
        {
            var command = ParseInternal(commandLine, isCaseSensitive);
            return new ConsoleCommand<TCommandCodeEnum>(command.CommandLine, command.IsCaseSensitive, command.Command, command.Parameters);
        }

        public static IConsoleCommand<TCommandCodeEnum> Parse<TCommandCodeEnum>(string commandLine, bool isCaseSensitive) where TCommandCodeEnum : struct
        {
            if (commandLine is null)
                throw new ArgumentNullException(nameof(commandLine));

            return ParseInternal<TCommandCodeEnum>(commandLine, isCaseSensitive);
        }

        public static IConsoleCommand<TCommandCodeEnum> TryParse<TCommandCodeEnum>(string commandLine, bool isCaseSensitive) where TCommandCodeEnum : struct
        {
            if (commandLine is null)
                return null;

            return ParseInternal<TCommandCodeEnum>(commandLine, isCaseSensitive);
        }

    }

}
