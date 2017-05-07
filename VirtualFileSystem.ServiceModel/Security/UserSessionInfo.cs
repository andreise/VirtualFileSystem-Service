using System;

namespace VirtualFileSystem.ServiceModel.Security
{

    internal sealed class UserSessionInfo
    {
        private DateTime lastActivityTimeUtc;

        public DateTime LastActivityTimeUtc
        {
            get { return this.lastActivityTimeUtc; }
            set { this.lastActivityTimeUtc = value.ToUniversalTime(); }
        }

        public byte[] Token { get; }

        public string CurrentDirectory { get; set; }

        public UserSessionInfo(DateTime lastActivityTime, byte[] token)
        {
            this.LastActivityTimeUtc = lastActivityTime;
            this.Token = token;
        }
    }

}
