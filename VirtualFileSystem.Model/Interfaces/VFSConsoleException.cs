using System;

namespace VirtualFileSystem.Model
{

    internal sealed class VFSConsoleException : Exception
    {
        public VFSConsoleException(string message) : base(message)
        {
        }
    }

}
