using static System.Console;

namespace VFSClient
{
    using Model;

    static class Program
    {
        static void Main(string[] args) => new VFSClient(ReadLine, WriteLine).Run().Wait();
    }
}
