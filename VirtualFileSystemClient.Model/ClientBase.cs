using System;
using System.ServiceModel;

namespace VirtualFileSystemClient.Model
{

    public abstract class ClientBase<TAuthorizeFault, TDeauthorizeFault, TCommandFault> :
        Common.ClientBase<FaultException<TAuthorizeFault>, FaultException<TDeauthorizeFault>, FaultException<TCommandFault>>
    {
        public ClientBase(Func<string> readLine, Action<string> writeLine) : base(readLine, writeLine)
        {
        }
    }

}
