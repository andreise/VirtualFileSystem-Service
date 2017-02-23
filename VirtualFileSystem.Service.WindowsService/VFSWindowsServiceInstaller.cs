using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace VirtualFileSystem.Service.WindowsService
{

    /// <summary>
    /// Class which allows the service to be installed by the InstallUtil.exe tool
    /// </summary>
    [RunInstaller(true)]
    public sealed class VFSWindowsServiceInstaller : Installer
    {

        private readonly ServiceProcessInstaller process;

        private readonly ServiceInstaller service;

        private VFSWindowsServiceInstaller(ServiceAccount account, string serviceName)
        {
            this.process = new ServiceProcessInstaller();
            this.process.Account = account;

            this.service = new ServiceInstaller();
            this.service.ServiceName = serviceName;

            this.Installers.Add(process);
            this.Installers.Add(service);
        }

        public VFSWindowsServiceInstaller() : this(
            account: CommonData.DefaultServiceAccount,
            serviceName: CommonData.DefaultServiceName
        )
        {
        }

    }

}
