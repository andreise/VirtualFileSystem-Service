using static System.Console;

namespace VirtualFileSystemClient
{
    using Model;

    static class Program
    {
        static void Main(string[] args) => VFSClientLegacy.Run(ReadLine, WriteLine).Wait();
    }
}
