using System;
using System.ServiceModel;
using System.ServiceProcess;

namespace VirtualFileSystem.Service
{

    public sealed class VFSWindowsService : ServiceBase
    {

        public const string DefaultServiceName = "Virtual File System Windows Service @ ASergeev";

        private ServiceHost ServiceHost { get; /*private*/ set; }

        private VFSWindowsService(string serviceName)
        {
            this.ServiceName = serviceName;
        }

        private VFSWindowsService() : this(serviceName: DefaultServiceName)
        {
        }

        private static void Main(string[] args)
        {
            Run(new VFSWindowsService());
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            this.CloseServiceHost();
            this.ServiceHost = new ServiceHost(typeof(VFSService));
            this.ServiceHost.Open();
        }

        protected override void OnStop()
        {
            base.OnStop();
            this.CloseServiceHost();
            this.ServiceHost = null;
        }

        private void CloseServiceHost()
        {
            if ((object)this.ServiceHost == null)
                return;

            switch (this.ServiceHost.State)
            {
                case CommunicationState.Opening:
                case CommunicationState.Opened:
                    this.ServiceHost.Close();
                    break;
            }
        }

    }

}
