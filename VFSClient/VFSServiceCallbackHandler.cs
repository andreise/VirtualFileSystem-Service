using System;
using static System.FormattableString;

namespace VFSClient
{
    using VFSServiceReference;

    internal sealed class VFSServiceCallbackHandler : IVFSServiceCallback
    {

        public VFSClientModel Owner { get; }

        public VFSServiceCallbackHandler(VFSClientModel owner)
        {
            this.Owner = owner;
        }

        public void FileSystemChangedNotify(FileSystemChangedData data)
        {
            if (data.UserName == this.Owner.UserInfo.UserName)
                return;

            Console.WriteLine(Invariant($"User '{data.UserName}' performs command: {data.CommandLine}."));
        }

    }

}
