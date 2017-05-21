using System;

namespace VirtualFileSystemClient.Model
{
    using VirtualFileSystemServiceReference;

    internal sealed class VFSServiceCallbackHandler : IVFSServiceCallback
    {

        private readonly Action<CommandPerformedData> commandPerformedHandler;

        public VFSServiceCallbackHandler(
            Action<CommandPerformedData> commandPerformedHandler
        )
        {
            this.commandPerformedHandler = commandPerformedHandler;
        }

        public void OnCommandPerformed(CommandPerformedData data) => this.commandPerformedHandler?.Invoke(data);

    }

}
