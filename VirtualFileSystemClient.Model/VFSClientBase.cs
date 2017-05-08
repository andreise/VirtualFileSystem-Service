using System;
using System.ServiceModel;

namespace VirtualFileSystemClient.Model
{

    public abstract class VFSClientBase<TConnectDetail, TDisconnectDetail, TVFSDetail> :
        Common.VFSClientBase<FaultException<TConnectDetail>, FaultException<TDisconnectDetail>, FaultException<TVFSDetail>>
    {

        public VFSClientBase(Action<string> output) : base(output)
        {
        }

    }

}
