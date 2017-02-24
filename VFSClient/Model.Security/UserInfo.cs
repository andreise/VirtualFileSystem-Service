namespace VFSClient.Model.Security
{

    internal sealed class UserInfo
    {
        public string UserName { get; }

        public byte[] Token { get; }

        public UserInfo(string userName, byte[] token)
        {
            this.UserName = userName;
            this.Token = token;
        }
    }

}
