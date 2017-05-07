namespace VFSClient.Model.Security
{

    public sealed class User
    {

        public UserCredentials Credentials { get; private set; }

        public void SetCredentials(string userName, byte[] token) =>
            this.Credentials = new UserCredentials(userName, token);

        public void ResetCredentials() =>
            this.Credentials = null;

    }

}
