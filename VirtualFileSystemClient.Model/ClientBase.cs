using System;
using System.ServiceModel;

namespace VirtualFileSystemClient.Model
{

    public abstract class ClientBase<TAuthorizeFault, TDeauthorizeFault, TFileSystemConsoleFault> :
        Common.ClientBase<FaultException<TAuthorizeFault>, FaultException<TDeauthorizeFault>, FaultException<TFileSystemConsoleFault>>
    {
        public ClientBase(Func<string> input, Action<string> output) : base(input, output)
        {
        }
    }

}
