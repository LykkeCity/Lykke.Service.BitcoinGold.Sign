using Lykke.Service.BitcoinGold.Sign.Core.Settings.ServiceSettings;
using Lykke.Service.BitcoinGold.Sign.Core.Settings.SlackNotifications;

namespace Lykke.Service.BitcoinGold.Sign.Core.Settings
{
    public class AppSettings
    {
        public BitcoinGoldSignSettings BitcoinGoldSign { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}
