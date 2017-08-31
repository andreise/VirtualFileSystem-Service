namespace VirtualFileSystemClient
{
    using Model;

    static class Program
    {
        static void Main(string[] args) => new Client().RunAsync().Wait();
    }
}
