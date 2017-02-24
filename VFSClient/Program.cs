namespace VFSClient
{
    static class Program
    {
        static void Main(string[] args) => new VFSClientModel().Run().Wait();
    }
}
