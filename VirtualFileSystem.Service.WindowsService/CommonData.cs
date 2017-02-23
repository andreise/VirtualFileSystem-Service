using System.ServiceProcess;

namespace VirtualFileSystem.Service.WindowsService
{

    static class CommonData
    {
        public const ServiceAccount DefaultServiceAccount = ServiceAccount.LocalSystem;

        public const string DefaultServiceName = "Virtual File System Windows Service";
    }

}
