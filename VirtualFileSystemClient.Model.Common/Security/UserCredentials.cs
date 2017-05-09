using System;

namespace VirtualFileSystemClient.Model.Common.Security
{

    public sealed class UserCredentials
    {

        public string UserName { get; }

        public byte[] Token { get; }

        public UserCredentials(string userName, byte[] token)
        {
            if (userName is null)
                throw new ArgumentNullException(paramName: nameof(userName));

            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException(paramName: nameof(userName), message: "User name is empty.");

            this.UserName = userName;
            this.Token = token;
        }

    }

}
