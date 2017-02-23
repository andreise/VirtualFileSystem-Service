using System.ServiceProcess;

namespace VirtualFileSystem.Service.WindowsService
{
    static class Program
    {
        static void Main(string[] args)
        {
            ServiceBase.Run(new VFSWindowsService());
        }
    }
}
