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
        [FaultContract(typeof(AuthorizeFault))]
        AuthorizeResponse Authorize(AuthorizeRequest request);

        [OperationContract]
        [FaultContract(typeof(DeauthorizeFault))]
        DeauthorizeResponse Deauthorize(DeauthorizeRequest request);

        [OperationContract]
        [FaultContract(typeof(FileSystemConsoleFault))]
        FileSystemConsoleResponse FileSystemConsole(FileSystemConsoleRequest request);
    }

}
