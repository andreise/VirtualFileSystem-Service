using System;
using System.ServiceModel;

namespace VirtualFileSystemClient.Model
{

    public abstract class VFSClientBase<TConnectFault, TDisconnectFault, TVFSFault> :
        Common.VFSClientBase<FaultException<TConnectFault>, FaultException<TDisconnectFault>, FaultException<TVFSFault>>
    {

        public VFSClientBase(Action<string> output) : base(output)
        {
        }

    }

}
