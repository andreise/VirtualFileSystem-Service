using System;

namespace VirtualFileSystem.Model
{

    internal sealed class VFSException : Exception
    {
        public VFSException(string message) : base(message)
        {
        }
    }

}
