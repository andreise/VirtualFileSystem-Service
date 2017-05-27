using System;

namespace VirtualFileSystem.ServiceModel.Security
{

    internal sealed class UserSessionInfo
    {

        private static DateTime UtcNow() => DateTime.UtcNow;

        private static void ValidateTimeUtc(DateTime value)
        {
            if (value.Kind != DateTimeKind.Utc)
                throw new ArgumentException("DateTime must be represented in UTC kind.", nameof(value));
        }

        private DateTime lastActivityTimeUtc;

        public DateTime LastActivityTimeUtc
        {
            get => this.lastActivityTimeUtc;
            private set
            {
                ValidateTimeUtc(value);
                this.lastActivityTimeUtc = value;
            }
        }

        public byte[] Token { get; }

        public string CurrentDirectory { get; set; }

        public UserSessionInfo(DateTime utcNow, byte[] token)
        {
            if (token is null)
                throw new ArgumentNullException(nameof(token));

            this.LastActivityTimeUtc = utcNow;
            this.Token = token;
        }

        public UserSessionInfo(byte[] token) : this(UtcNow(), token)
        {
        }

        public void UpdateLastActivityTimeUtc(DateTime utcNow) => this.LastActivityTimeUtc = utcNow;

        public void UpdateLastActivityTimeUtc() => this.UpdateLastActivityTimeUtc(UtcNow());

        public bool IsActualSession(DateTime utcNow, TimeSpan timeout)
        {
            ValidateTimeUtc(utcNow);
            return
                utcNow >= this.lastActivityTimeUtc &&
                utcNow - this.lastActivityTimeUtc <= timeout;
        }

        public bool IsActualSession(TimeSpan timeout) => this.IsActualSession(UtcNow(), timeout);

    }

}
