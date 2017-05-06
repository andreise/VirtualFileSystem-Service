using System;

namespace VirtualFileSystem.Model
{

    public sealed class VFSException : Exception
    {
        public VFSException(string message) : base(message)
        {
        }
    }

}
