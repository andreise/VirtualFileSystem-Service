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

        private VFSWindowsServiceInstaller(ServiceAccount account, string serviceName)
        {
            this.Installers.Add(new ServiceProcessInstaller() { Account = account });
            this.Installers.Add(new ServiceInstaller() { ServiceName = serviceName });
        }

        public VFSWindowsServiceInstaller() : this(
            account: VFSWindowsServiceConfiguration.ServiceAccount,
            serviceName: VFSWindowsServiceConfiguration.ServiceName
        )
        {
        }

    }

}
