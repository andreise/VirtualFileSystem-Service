using System;

namespace VirtualFileSystem.ServiceModel.Security
{

    internal sealed class UserSessionInfo
    {

        private static void ValidateDateTime(DateTime dateTime)
        {
            if (dateTime.Kind != DateTimeKind.Utc)
                throw new ArgumentException("DateTime must be represented in UTC kind.", nameof(dateTime));
        }

        private DateTime lastActivityTimeUtc;

        public DateTime LastActivityTimeUtc
        {
            get => this.lastActivityTimeUtc;
            set
            {
                ValidateDateTime(value);
                this.lastActivityTimeUtc = value;
            }
        }

        public byte[] Token { get; }

        public string CurrentDirectory { get; set; }

        public UserSessionInfo(DateTime lastActivityTimeUtc, byte[] token)
        {
            this.LastActivityTimeUtc = lastActivityTimeUtc;
            this.Token = token;
        }

        public bool IsActualSession(TimeSpan timeout, DateTime nowUtc)
        {
            ValidateDateTime(nowUtc);
            return
                nowUtc >= this.lastActivityTimeUtc &&
                nowUtc - this.lastActivityTimeUtc <= timeout;
        }

        public bool IsActualSession(TimeSpan timeout) => this.IsActualSession(timeout, DateTime.UtcNow);

    }

}
