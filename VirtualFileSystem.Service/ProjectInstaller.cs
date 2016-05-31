using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace VirtualFileSystem.Service
{

    /// <summary>
    /// Class which allows the service to be installed by the InstallUtil.exe tool
    /// </summary>
    [RunInstaller(true)]
    public sealed class ProjectInstaller : Installer
    {

        private readonly ServiceProcessInstaller process;
        private readonly ServiceInstaller service;

        public ProjectInstaller(ServiceAccount account, string serviceName)
        {
            this.process = new ServiceProcessInstaller();
            this.process.Account = account;
            this.service = new ServiceInstaller();
            this.service.ServiceName = serviceName;
            this.Installers.Add(process);
            this.Installers.Add(service);
        }

        public ProjectInstaller() : this(account: ServiceAccount.LocalSystem, serviceName: VFSWindowsService.DefaultServiceName)
        {
        }

    }

}
