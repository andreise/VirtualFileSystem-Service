using System;

namespace VFSClient.Model
{
    using VFSServiceReference;

    internal sealed class VFSServiceCallbackHandler : IVFSServiceCallback
    {

        private readonly Action<FileSystemChangedData> handler;

        public VFSServiceCallbackHandler(Action<FileSystemChangedData> handler)
        {
            if ((object)handler == null)
                throw new ArgumentNullException(nameof(handler));

            this.handler = handler;
        }

        public void FileSystemChangedNotify(FileSystemChangedData data) => this.handler(data);

    }

}
