using System;

namespace VirtualFileSystemClient.Model
{
    using VirtualFileSystemServiceReference;

    internal sealed class VFSServiceCallbackHandler : IVFSServiceCallback
    {

        private readonly Action<FileSystemConsoleNotificationData> handler;

        public VFSServiceCallbackHandler(Action<FileSystemConsoleNotificationData> handler)
        {
            if (handler is null)
                throw new ArgumentNullException(nameof(handler));

            this.handler = handler;
        }

        public void Notify(FileSystemConsoleNotificationData data) => this.handler(data);

    }

}
