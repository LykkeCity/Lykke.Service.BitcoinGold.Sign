using Autofac;
using Lykke.BitcoinGold.Sign.Services.Sign;
using Lykke.BitcoinGold.Sign.Services.Wallet;
using Lykke.Service.BitcoinGold.Sign.Core;
using Lykke.Service.BitcoinGold.Sign.Core.Settings.ServiceSettings;
using Lykke.Service.BitcoinGold.Sign.Core.Sign;
using Lykke.SettingsReader;
using NBitcoin;

namespace Lykke.BitcoinGold.Sign.Services
{
    public  class ServiceModule:Module
    {
        private readonly IReloadingManager<BitcoinGoldSignSettings> _settings;
        public ServiceModule(IReloadingManager<BitcoinGoldSignSettings> settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterNetwork(builder);
            
            builder.RegisterType<TransactionSigningService>().As<ITransactionSigningService>();
            builder.RegisterType<WalletGenerator>().As<IWalletGenerator>();
        }

        private void RegisterNetwork(ContainerBuilder builder)
        {
            NBitcoin.Altcoins.BGold.Instance.EnsureRegistered();
            var network = _settings.CurrentValue.Network?.ToLower() == "main" ?
                NBitcoin.Altcoins.BGold.Instance.Mainnet :
                NBitcoin.Altcoins.BGold.Instance.Testnet;
            builder.RegisterInstance(network).As<Network>();
        }
    }
}
