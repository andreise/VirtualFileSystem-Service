using System;
using System.ServiceModel;

namespace VirtualFileSystemClient.Model
{

    public abstract class VFSClient<TConnectDetail, TDisconnectDetail, TVFSDetail> :
        Common.VFSClient<FaultException<TConnectDetail>, FaultException<TDisconnectDetail>, FaultException<TVFSDetail>>
    {

        public VFSClient(Action<string> output) : base(output)
        {
        }

    }

}
