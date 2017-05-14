using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace VirtualFileSystem.Common.Console
{

    /// <summary>
    /// Console Command
    /// </summary>
    public class ConsoleCommand : IConsoleCommand
    {

        /// <summary>
        /// Original Command Line
        /// </summary>
        public string OriginalCommandLine { get; }

        /// <summary>
        /// Command Line
        /// </summary>
        /// <remarks>Command Line in the normalized form</remarks>
        public string CommandLine { get; }

        /// <summary>
        /// Is case sensitive command line
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
        /// <param name="isCaseSensitive">Is case sensitive command line</param>
        /// <exception cref="ArgumentNullException">Throws if command line is null</exception>
        public ConsoleCommand(string commandLine, bool isCaseSensitive)
        {
            if (commandLine is null)
                throw new ArgumentNullException(nameof(commandLine));

            this.OriginalCommandLine = commandLine;

            var items = commandLine.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            this.Command = items.FirstOrDefault() ?? string.Empty;
            this.Parameters = new ReadOnlyCollection<string>(items.Skip(1).ToArray());

            this.CommandLine = string.Join(" ", new string[] { this.Command }.Concat(this.Parameters));
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
