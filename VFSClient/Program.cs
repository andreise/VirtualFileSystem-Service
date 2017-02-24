namespace VFSClient
{
    using Model;

    static class Program
    {
        static void Main(string[] args) => new VFSClient().Run().Wait();
    }
}
