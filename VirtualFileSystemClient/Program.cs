using System.Threading.Tasks;

namespace VirtualFileSystemClient
{
    using Model;

    static class Program
    {
		static async Task MainAsync(string[] args) => await new Client().RunAsync();

		static void Main(string[] args) => MainAsync(args).GetAwaiter().GetResult();
    }
}
