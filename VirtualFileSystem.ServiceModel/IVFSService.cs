using System.Runtime.Serialization;
using System.ServiceModel;

[assembly: ContractNamespace("http://andrey.sergeev.vfsservice", ClrNamespace = "VirtualFileSystem.ServiceModel")]

namespace VirtualFileSystem.ServiceModel
{

    /// <summary>
    /// Virtual File System Service
    /// </summary>
    [ServiceContract(
        CallbackContract = typeof(IVFSServiceCallback)
        //, SessionMode = SessionMode.Required
    )]
    public interface IVFSService
    {
        [OperationContract]
        [FaultContract(typeof(ConnectFault))]
        ConnectResponse Connect(ConnectRequest request);

        [OperationContract]
        [FaultContract(typeof(DisconnectFault))]
        DisconnectResponse Disconnect(DisconnectRequest request);

        [OperationContract]
        [FaultContract(typeof(FSCommandFault))]
        FSCommandResponse FSCommand(FSCommandRequest request);
    }

}
