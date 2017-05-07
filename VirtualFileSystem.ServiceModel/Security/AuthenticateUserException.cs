using System;

namespace VirtualFileSystem.Service.Model.Security
{
    internal sealed class AuthenticateUserException : Exception
    {
        public AuthenticateUserException(string message) : base(message)
        {
        }
    }
}
