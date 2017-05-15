using System.Collections.Generic;

namespace VirtualFileSystem.Common.Console
{

    /// <summary>
    /// Console Command
    /// </summary>
    public interface IConsoleCommand
    {

        /// <summary>
        /// Original Command Line
        /// </summary>
        string OriginalCommandLine { get; }

        /// <summary>
        /// Command Line
        /// </summary>
        /// <remarks>Command Line in the normalized form</remarks>
        string CommandLine { get; }

        /// <summary>
        /// Command
        /// </summary>
        string Command { get; }

        /// <summary>
        /// Command Parameters
        /// </summary>
        IReadOnlyList<string> Parameters { get; }

        /// <summary>
        /// Is case sensitive command line
        /// </summary>
        bool IsCaseSensitive { get; }

        /// <summary>
        /// Test command to comform with the specified command
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Returns true if the command is comforms to the specified command, otherwise returns false</returns>
        /// <exception cref="ArgumentNullException">Throws if the specified command is null</exception>
        bool IsCommand(string command);

    }

}
