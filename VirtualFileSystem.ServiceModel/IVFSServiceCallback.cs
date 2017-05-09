using System.ServiceModel;

namespace VirtualFileSystem.ServiceModel
{

    internal interface IVFSServiceCallback
    {

        [OperationContract(IsOneWay = true)]
        void Notify(FileSystemConsoleNotificationData data);

    }

}
