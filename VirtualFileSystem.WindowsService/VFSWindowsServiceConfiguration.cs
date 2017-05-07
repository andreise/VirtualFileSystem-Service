using System.ServiceProcess;

namespace VirtualFileSystem.Service.WindowsService
{

    static class VFSWindowsServiceConfiguration
    {

        private const ServiceAccount DefaultServiceAccount = ServiceAccount.LocalSystem;

        private const string DefaultServiceName = "Virtual File System Windows Service";

        public static ServiceAccount ServiceAccount => DefaultServiceAccount;

        public static string ServiceName => DefaultServiceName;

    }

}
