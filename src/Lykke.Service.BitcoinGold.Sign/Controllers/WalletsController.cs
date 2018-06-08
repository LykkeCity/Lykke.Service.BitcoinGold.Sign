using Common;
using Lykke.Service.BitcoinGold.Sign.Core;
using Lykke.Service.BitcoinGold.Sign.Models.Wallet;
using Microsoft.AspNetCore.Mvc;
using NBitcoin;

namespace Lykke.Service.BitcoinGold.Sign.Controllers
{
    [Route("api/[controller]")]
    public class WalletsController
    {
        private readonly IWalletGenerator _walletGenerator;

        public WalletsController(IWalletGenerator walletGenerator)
        {
            _walletGenerator = walletGenerator;
        }

        [HttpPost]
        public WalletCreationResponse CreateWallet()
        {
            var wallet = _walletGenerator.Generate();

            return new WalletCreationResponse
            {
                PublicAddress = wallet.Address,
                PrivateKey = wallet.PrivateKey,
                AddressContext = new AddressContextContract { PubKey = wallet.PubKey }.ToJson()
            };
        }
    }
}
