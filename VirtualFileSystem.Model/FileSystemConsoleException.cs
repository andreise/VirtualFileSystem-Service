using System;

namespace VirtualFileSystem.Model
{

    public sealed class FileSystemConsoleException : Exception
    {
        public FileSystemConsoleException(string message) : base(message)
        {
        }
    }

}
