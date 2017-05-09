using System;

namespace VirtualFileSystem.Model.Console
{

    public sealed class FileSystemConsoleException : Exception
    {
        public FileSystemConsoleException(string message) : base(message)
        {
        }
    }

}
