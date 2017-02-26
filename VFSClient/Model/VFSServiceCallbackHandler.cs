using System;
using static System.FormattableString;

namespace VFSClient.Model
{
    using VFSServiceReference;

    internal sealed class VFSServiceCallbackHandler : IVFSServiceCallback
    {

        public VFSClient Owner { get; }

        public VFSServiceCallbackHandler(VFSClient owner)
        {
            if ((object)owner == null)
                throw new ArgumentNullException(nameof(owner));

            this.Owner = owner;
        }

        public void FileSystemChangedNotify(FileSystemChangedData data)
        {
            if ((object)data == null)
                return;

            if (data.UserName == this.Owner.UserInfo.UserName)
                return;

            Console.WriteLine(Invariant($"User '{data.UserName}' performs command: {data.CommandLine}."));
        }

    }

}
