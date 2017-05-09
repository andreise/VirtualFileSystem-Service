using System;

namespace VirtualFileSystem.Model
{

    public sealed class FileSystemException : Exception
    {
        public FileSystemException(string message) : base(message)
        {
        }
    }

}
