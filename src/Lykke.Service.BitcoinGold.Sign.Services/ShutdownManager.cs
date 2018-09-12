using System.Threading.Tasks;
using Lykke.Service.BitcoinGold.Sign.Core.Services;

namespace Lykke.BitcoinGold.Sign.Services
{
    public class ShutdownManager : IShutdownManager
    {
        public async Task StopAsync()
        {
            // TODO: Implement your shutdown logic here. Good idea is to log every step

            await Task.CompletedTask;
        }
    }
}
