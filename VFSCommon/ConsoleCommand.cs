using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace VFSCommon
{

    /// <summary>
    /// Console Command
    /// </summary>
    public class ConsoleCommand
    {
        /// <summary>
        /// Command Line
        /// </summary>
        public string CommandLine { get; }

        /// <summary>
        /// Is command a case sensitive
        /// </summary>
        public bool IsCaseSensitive { get; }

        /// <summary>
        /// Command
        /// </summary>
        public string Command { get; }

        /// <summary>
        /// Command Parameters
        /// </summary>
        public IReadOnlyList<string> Parameters { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="commandLine">Command Line</param>
        /// <param name="isCaseSensitive">Is command a case sensitive</param>
        /// <param name="command">Command</param>
        /// <param name="parameters">Command Parameters</param>
        protected ConsoleCommand(string commandLine, bool isCaseSensitive, string command, IEnumerable<string> parameters = null)
        {
            Func<string, string> normalizeString = value => value?.Trim() ?? string.Empty;

            this.CommandLine = normalizeString(commandLine);
            this.IsCaseSensitive = isCaseSensitive;
            this.Command = normalizeString(command);
            this.Parameters = new ReadOnlyCollection<string>(parameters?.Where(item => !string.IsNullOrWhiteSpace(item)).ToArray() ?? new string[0]);
        }

        /// <summary>
        /// Parses the command from a command line
        /// </summary>
        /// <param name="commandLine">Command Line</param>
        /// <param name="isCaseSensitive">Is command a case sensitive</param>
        /// <returns>Parsed command instance</returns>
        /// <exception cref="NullReferenceException">Throws if the command line is null</exception>
        protected static ConsoleCommand ParseInternal(string commandLine, bool isCaseSensitive)
        {
            var commandLineItems = commandLine.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            return new ConsoleCommand(
                commandLine,
                isCaseSensitive,
                commandLineItems.FirstOrDefault() ?? string.Empty,
                commandLineItems.Skip(1)
            );
        }

        /// <summary>
        /// Parses the command from a command line
        /// </summary>
        /// <param name="commandLine">Command Line</param>
        /// <param name="isCaseSensitive">Is command a case sensitive</param>
        /// <returns>A parsed command instance</returns>
        /// <exception cref="ArgumentNullException">Throws if the command line is null</exception>
        public static ConsoleCommand Parse(string commandLine, bool isCaseSensitive)
        {
            if ((object)commandLine == null)
                throw new ArgumentNullException(nameof(commandLine));

            return ParseInternal(commandLine, isCaseSensitive);
        }

        /// <summary>
        /// Parses the command from a command line
        /// </summary>
        /// <param name="commandLine">Command Line</param>
        /// <param name="isCaseSensitive">Is command a case sensitive</param>
        /// <returns>A parsed command instance or null if the command line is null</returns>
        public static ConsoleCommand ParseNullable(string commandLine, bool isCaseSensitive)
        {
            if ((object)commandLine == null)
                return null;

            return ParseInternal(commandLine, isCaseSensitive);
        }

        /// <summary>
        /// Test command to comform with the specified command
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Returns true if the command is comforms to the specified command, otherwise returns false</returns>
        /// <exception cref="ArgumentNullException">Throws if the specified command is null</exception>
        public bool IsCommand(string command)
        {
            if ((object)command == null)
                throw new ArgumentNullException(nameof(command));

            return string.Equals(
                this.Command,
                command.Trim(),
                this.IsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase
            );
        }
    }

}
