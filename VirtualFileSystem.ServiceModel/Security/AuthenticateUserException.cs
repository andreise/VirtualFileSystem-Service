using System;

namespace VirtualFileSystem.ServiceModel.Security
{

    [Serializable]
    internal sealed class AuthenticateUserException : Exception
    {
        public AuthenticateUserException(string message) : base(message)
        {
        }
    }

}
