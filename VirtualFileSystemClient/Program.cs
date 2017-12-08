using System.Threading.Tasks;

namespace VirtualFileSystemClient
{
    using Model;

    static class Program
    {
        static async Task Main(string[] args) => await new Client().RunAsync();
    }
}
