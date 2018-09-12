using System.Collections.Generic;
using System.Linq;
using Lykke.BitcoinGold.Sign.Services.Sign;
using NBitcoin;
using NBitcoin.Altcoins;
using NBitcoin.JsonConverters;
using Xunit;

namespace Lykke.Service.BitcoinGold.Sign.Tests
{
    public class SignTests
    {
        [Fact]
        public void Can_Sign_Mainnet_transaction()
        {
            var network = BGold.Instance.Mainnet;

            var key = new Key();
            var addr = key.PubKey.WitHash.ScriptPubKey.Hash.GetAddress(network);
            var spentTx = new Transaction()
            {
                Outputs = { new TxOut("1", addr) }
            };

            var builder = new TransactionBuilder();
            builder.SetConsensusFactory(network);
            builder.AddCoins(spentTx);
            builder.Send(addr, "0.001");
            builder.SendFees("0.001");
            builder.SetChange(addr);
            builder.AddKeys(key);

            var signedTx = builder.BuildTransaction(true);
            AssertCorrectlySigned(signedTx, addr.ScriptPubKey, spentTx.Outputs[0].Value);

            var unsignedTx = builder.BuildTransaction(false);
            var txInfo = new TransactionInfo
            {
                TransactionHex = unsignedTx.ToHex(),
                UsedCoins = builder.FindSpentCoins(unsignedTx).OfType<Coin>()
            };


            var signer = new TransactionSigningService(network);
            var signedTx2 = signer.Sign(Serializer.ToString(txInfo), new List<string> { key.ToString(network) });

            Assert.Equal(signedTx.ToHex(), signedTx2.TransactionHex);

        }

        #region Helper 
        private static void AssertCorrectlySigned(Transaction tx, Script scriptPubKey, Money money)
        {
            for (int i = 0; i < tx.Inputs.Count; i++)
            {
                Assert.True(tx.Inputs[i].ScriptSig.Length > 0);

                Assert.True(Script.VerifyScript(scriptPubKey, tx, 0, money, ScriptVerify.ForkId, SigHash.All));
            }
        }
        #endregion
    }

}
