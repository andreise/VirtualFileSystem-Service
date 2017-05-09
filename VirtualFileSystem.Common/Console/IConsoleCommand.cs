using System.Collections.Generic;

namespace VirtualFileSystem.Common.Console
{

    /// <summary>
    /// Console Command
    /// </summary>
    public interface IConsoleCommand
    {

        /// <summary>
        /// Command Line
        /// </summary>
        string CommandLine { get; }

        /// <summary>
        /// Is command a case sensitive
        /// </summary>
        bool IsCaseSensitive { get; }

        /// <summary>
        /// Command
        /// </summary>
        string Command { get; }

        /// <summary>
        /// Command Parameters
        /// </summary>
        IReadOnlyList<string> Parameters { get; }

        /// <summary>
        /// Test command to comform with the specified command
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Returns true if the command is comforms to the specified command, otherwise returns false</returns>
        /// <exception cref="ArgumentNullException">Throws if the specified command is null</exception>
        bool IsCommand(string command);

    }

}
