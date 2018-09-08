using System.Threading.Tasks;
using Lykke.Service.BitcoinGold.Sign.Core.Services;

namespace Lykke.BitcoinGold.Sign.Services
{
    public class StartupManager : IStartupManager
    {
        public async Task StartAsync()
        {
            await Task.CompletedTask;
        }
    }
}
