using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;

namespace VirtualFileSystem.Service.WindowsService
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceBase.Run(new VFSWindowsService());
        }
    }
}
