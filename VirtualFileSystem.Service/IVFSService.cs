using System.ServiceModel;

namespace VirtualFileSystem.Service
{

    [ServiceContract(Name = "VFSService", Namespace = "http://andrey.sergeev.vfsservice", SessionMode = SessionMode.Required)]
    public interface IVFSService
    {

        [OperationContract]
        void Foo();

    }

}
