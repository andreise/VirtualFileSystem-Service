namespace VFSClient.Model.Security
{

    internal sealed class UserCredentials
    {

        public string UserName { get; }

        public byte[] Token { get; }

        public UserCredentials(string userName, byte[] token)
        {
            this.UserName = userName;
            this.Token = token;
        }

    }

}
