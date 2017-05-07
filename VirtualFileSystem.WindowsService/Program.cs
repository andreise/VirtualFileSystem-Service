using System.ServiceProcess;

namespace VirtualFileSystem.WindowsService
{
    static class Program
    {
        static void Main(string[] args) => ServiceBase.Run(new VFSWindowsService());
    }
}
