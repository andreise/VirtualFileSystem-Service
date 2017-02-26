using System.ServiceModel;

namespace VirtualFileSystem.Service.Model
{

    internal interface IVFSServiceCallback
    {

        [OperationContract(IsOneWay = true)]
        void FileSystemChangedNotify(FileSystemChangedData data);

    }

}
