using System.ServiceModel;
using System.ServiceProcess;

namespace VirtualFileSystem.Service.WindowsService
{
    using Model;

    public sealed class VFSWindowsService : ServiceBase
    {

        private ServiceHost serviceHost;

        private VFSWindowsService(string serviceName)
        {
            this.ServiceName = serviceName;
        }

        public VFSWindowsService() : this(serviceName: CommonData.DefaultServiceName)
        {
        }

        private void CloseServiceHost()
        {
            if ((object)this.serviceHost == null)
                return;

            switch (this.serviceHost.State)
            {
                case CommunicationState.Opening:
                case CommunicationState.Opened:
                    this.serviceHost.Close();
                    break;
            }
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            this.CloseServiceHost();
            this.serviceHost = new ServiceHost(typeof(VFSService));
            this.serviceHost.Open();
        }

        protected override void OnStop()
        {
            base.OnStop();
            this.CloseServiceHost();
            this.serviceHost = null;
        }

    }

}
