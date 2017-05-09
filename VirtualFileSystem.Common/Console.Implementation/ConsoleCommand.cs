using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace VirtualFileSystem.Common.Console.Implementation
{

    /// <summary>
    /// Console Command
    /// </summary>
    internal class ConsoleCommand : IConsoleCommand
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
        public ConsoleCommand(string commandLine, bool isCaseSensitive, string command, IEnumerable<string> parameters = null)
        {
            string NormalizeString(string value) => value?.Trim() ?? string.Empty;

            this.CommandLine = NormalizeString(commandLine);
            this.IsCaseSensitive = isCaseSensitive;
            this.Command = NormalizeString(command);
            this.Parameters = new ReadOnlyCollection<string>(
                parameters?.Where(item => !string.IsNullOrWhiteSpace(item)).Select(item => item.Trim()).ToArray() ?? new string[0]
            );
        }

        /// <summary>
        /// Test command to comform with the specified command
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Returns true if the command is comforms to the specified command, otherwise returns false</returns>
        /// <exception cref="ArgumentNullException">Throws if the specified command is null</exception>
        public bool IsCommand(string command)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));

            return string.Equals(
                this.Command,
                command.Trim(),
                this.IsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase
            );
        }

    }

}
