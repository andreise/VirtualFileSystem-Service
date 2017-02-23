using System;
using System.Configuration;
using System.ServiceProcess;

namespace VirtualFileSystem.Service.WindowsService
{

    static class VFSWindowsServiceConfiguration
    {

        private static string GetSetting(string name)
        {
            try
            {
                return ConfigurationManager.AppSettings[name];
            }
            catch
            {
                return null;
            }
        }

        public static ServiceAccount ServiceAccount
        {
            get
            {
                const ServiceAccount DefaultServiceAccount = ServiceAccount.LocalSystem;
                try
                {
                    var result = (ServiceAccount)Enum.Parse(
                        typeof(ServiceAccount),
                        GetSetting("vfsservice:windowsservice:ServiceAccount"),
                        ignoreCase: true
                    );
                    return typeof(ServiceAccount).IsEnumDefined(result) ? result : DefaultServiceAccount;
                }
                catch
                {
                    return DefaultServiceAccount;
                }
            }
        }

        public static string ServiceName
        {
            get
            {
                const string DefaultServiceName = "Virtual File System Windows Service";
                try
                {
                    var result = GetSetting("vfsservice:windowsservice:DefaultServiceName");
                    return !string.IsNullOrWhiteSpace(result) ? result.Trim() : DefaultServiceName;
                }
                catch
                {
                    return DefaultServiceName;
                }
            }
        }

    }

}
