namespace Lykke.Service.BitcoinGold.Sign.Core
{
    public interface IGeneratedWallet
    {
        string Address { get; }
        string PrivateKey { get; }
        string PubKey { get; set; }
    }
    public interface IWalletGenerator
    {
        IGeneratedWallet Generate();
    }
}
