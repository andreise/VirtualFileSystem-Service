using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static System.FormattableString;

namespace VFSCommon
{

    /// <summary>
    /// Console Command
    /// </summary>
    public class BaseConsoleCommand
    {
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
        /// <param name="command">Command</param>
        /// <param name="parameters">Command Parameters</param>
        /// <exception cref="ArgumentNullException">Throws if the command is null</exception>
        /// <exception cref="ArgumentException">Throws if the command is empty or any command parameter is null or empty</exception>
        public BaseConsoleCommand(string command, string[] parameters = null)
        {
            if ((object)command == null)
                throw new ArgumentNullException(paramName: nameof(command));

            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentException(paramName: nameof(command), message: Invariant($"{nameof(command)} is empty."));

            if ((object)parameters != null)
                if (parameters.Any(item => string.IsNullOrWhiteSpace(item)))
                    throw new ArgumentException(paramName: nameof(parameters), message: Invariant($"Some {nameof(parameters)} item is null or empty."));

            this.Command = command.Trim();

            this.Parameters = new ReadOnlyCollection<string>(parameters ?? new string[0]);
        }

        /// <summary>
        /// Parses the command from a command line
        /// </summary>
        /// <param name="commandLine">Client command from a command line</param>
        /// <returns>A client command instance</returns>
        /// <exception cref="ArgumentNullException">Throws if the command is null</exception>
        /// <exception cref="ArgumentException">Throws if the command is empty</exception>
        public static BaseConsoleCommand Parse(string commandLine)
        {
            if ((object)commandLine == null)
                throw new ArgumentNullException(paramName: nameof(commandLine));

            if (string.IsNullOrWhiteSpace(commandLine))
                throw new ArgumentException(paramName: nameof(commandLine), message: Invariant($"{nameof(commandLine)} is empty."));

            string[] tempItems = commandLine.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

            List<string> items = new List<string>(tempItems.Length);
            for (int i = 0; i < tempItems.Length; i++)
                if (!string.IsNullOrWhiteSpace(tempItems[i]))
                    items.Add(tempItems[i].Trim());

            string command = items[0];
            string[] parameters = new string[items.Count - 1];
            items.CopyTo(
                index: 1,
                array: parameters,
                arrayIndex: 0,
                count: items.Count - 1
            );

            return new BaseConsoleCommand(command, parameters);
        }

        /// <summary>
        /// Test command to comform with the specified command
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Returns true if the command is comforms to the specified command, otherwise returns false</returns>
        /// <exception cref="ArgumentNullException">Throws if the specified command is null</exception>
        /// <exception cref="ArgumentException">Throws if the specified command is empty</exception>
        public bool IsCommand(string command)
        {
            if ((object)command == null)
                throw new ArgumentNullException(paramName: nameof(command));

            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentException(paramName: nameof(command), message: Invariant($"Some {nameof(command)} item is empty."));

            return string.Equals(this.Command, command.Trim(), StringComparison.OrdinalIgnoreCase);
        }
    }

}
