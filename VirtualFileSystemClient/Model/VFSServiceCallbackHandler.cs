using System;

namespace VirtualFileSystemClient.Model
{
    using VFSServiceReference;

    internal sealed class VFSServiceCallbackHandler : IVFSServiceCallback
    {

        private readonly Action<FileSystemChangedData> handler;

        public VFSServiceCallbackHandler(Action<FileSystemChangedData> handler)
        {
            if (handler is null)
                throw new ArgumentNullException(nameof(handler));

            this.handler = handler;
        }

        public void FileSystemChangedNotify(FileSystemChangedData data) => this.handler(data);

    }

}
