using Lykke.Service.BitcoinGold.Sign.Core;
using NBitcoin;

namespace Lykke.BitcoinGold.Sign.Services.Wallet
{
    internal class GeneratedWallet : IGeneratedWallet
    {
        public string Address { get; set; }
        public string PrivateKey { get; set; }
        public string PubKey { get; set; }
    }

    public class WalletGenerator: IWalletGenerator
    {
        private readonly Network _network;

        public WalletGenerator(Network network)
        {
            _network = network;
        }

        public IGeneratedWallet Generate()
        {
            var key = new Key();

            return new GeneratedWallet
            {
                Address = key.PubKey.WitHash.ScriptPubKey.Hash.GetAddress(_network).ToString(),
                PrivateKey = key.GetWif(_network).ToString(),
                PubKey = key.PubKey.ToString()
            };      
        }
    }
}
