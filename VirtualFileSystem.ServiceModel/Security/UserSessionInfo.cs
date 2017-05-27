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

        public DateTime LastActivityTimeUtc { get; private set; }

        public void UpdateLastActivityTimeUtc(DateTime utcNow)
        {
            ValidateTimeUtc(utcNow);
            this.LastActivityTimeUtc = utcNow;
        }

        public void UpdateLastActivityTimeUtc() =>
            this.UpdateLastActivityTimeUtc(UtcNow());

        public byte[] Token { get; }

        public string CurrentDirectory { get; set; }

        public UserSessionInfo(DateTime utcNow, byte[] token)
        {
            if (token is null)
                throw new ArgumentNullException(nameof(token));

            this.UpdateLastActivityTimeUtc(utcNow);
            this.Token = token;
        }

        public UserSessionInfo(byte[] token) : this(UtcNow(), token)
        {
        }

        public bool IsActualSession(DateTime utcNow, TimeSpan timeout)
        {
            ValidateTimeUtc(utcNow);
            return
                utcNow >= this.LastActivityTimeUtc &&
                utcNow - this.LastActivityTimeUtc <= timeout;
        }

        public bool IsActualSession(TimeSpan timeout) =>
            this.IsActualSession(UtcNow(), timeout);

    }

}
