using System.Threading.Tasks;

namespace Lykke.Service.BitcoinGold.Sign.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}
