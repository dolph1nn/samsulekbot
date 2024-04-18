using System.Threading.Tasks;

namespace SSB
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await Discord.DiscordHandler.Init();
        }
    }
}
