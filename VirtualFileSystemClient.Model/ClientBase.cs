using System;
using System.ServiceModel;

namespace VirtualFileSystemClient.Model
{
    using VirtualFileSystemClient.ModelCore;

    public abstract class ClientBase<TAuthorizeFault, TDeauthorizeFault, TCommandFault> :
        ClientCoreBase<FaultException<TAuthorizeFault>, FaultException<TDeauthorizeFault>, FaultException<TCommandFault>>
    {
        public ClientBase(Func<string> readLine, Action<string> writeLine) : base(readLine, writeLine)
        {
        }
    }

}
