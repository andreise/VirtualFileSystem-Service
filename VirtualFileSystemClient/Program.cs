using static System.Console;

namespace VirtualFileSystemClient
{
    using Model;

    static class Program
    {
        static void Main(string[] args) => new VFSClient(ReadLine, WriteLine).Run().Wait();
    }
}
