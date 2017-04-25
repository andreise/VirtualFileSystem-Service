using static System.Console;

namespace VFSClient
{
    using Model;

    static class Program
    {
        static void Main(string[] args) => VFSClient.Run(ReadLine, WriteLine).Wait();
    }
}
