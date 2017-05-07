using System;

namespace VirtualFileSystem.ServiceModel.Security
{
    internal sealed class AuthenticateUserException : Exception
    {
        public AuthenticateUserException(string message) : base(message)
        {
        }
    }
}
