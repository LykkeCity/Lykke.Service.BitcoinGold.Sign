using System.Collections.Generic;
using System.Linq;
using Lykke.Service.BitcoinGold.Sign.Core.Exceptions;
using Lykke.Service.BitcoinGold.Sign.Core.Sign;
using NBitcoin;
using NBitcoin.Altcoins;
using NBitcoin.JsonConverters;

namespace Lykke.BitcoinGold.Sign.Services.Sign
{
    internal class SignResult : ISignResult
    {
        public string TransactionHex { get; private set; }


        public static SignResult Ok(string signedHex)
        {
            return new SignResult
            {
                TransactionHex = signedHex
            };
        }
    }

    public class TransactionInfo
    {
        public string TransactionHex { get; set; }

        public IEnumerable<Coin> UsedCoins { get; set; }
    }

    public class TransactionSigningService : ITransactionSigningService
    {
        private readonly Network _network;
        public TransactionSigningService(Network network)
        {
            _network = network;
        }

        public ISignResult Sign(string transactionContext, IEnumerable<string> privateKeys)
        {
            var context = Serializer.ToObject<TransactionInfo>(transactionContext);

            var tx = CreateForkTransaction(new Transaction(context.TransactionHex));


            var secretKeys = privateKeys.Select(p => Key.Parse(p, _network)).ToList();

            Key GetPrivateKey(TxDestination pubKeyHash)
            {
                foreach (var secret in secretKeys)
                {
                    var key = new BitcoinSecret(secret, _network);
                    if (key.PubKey.Hash == pubKeyHash || key.PubKey.WitHash.ScriptPubKey.Hash == pubKeyHash)
                        return key.PrivateKey;
                }

                return null;
            }

            SigHash hashType = (SigHash)((uint)SigHash.All | 64U);

            for (int i = 0; i < tx.Inputs.Count; i++)
            {
                var input = tx.Inputs[i];

                var output = context.UsedCoins.FirstOrDefault(o => o.Outpoint == input.PrevOut)?.TxOut;

                if (output == null)
                    throw new BusinessException("Input not found", ErrorCode.InputNotFound);

                if (PayToPubkeyHashTemplate.Instance.CheckScriptPubKey(output.ScriptPubKey))
                {
                    var secret = GetPrivateKey(PayToPubkeyHashTemplate.Instance.ExtractScriptPubKeyParameters(output.ScriptPubKey));
                    if (secret != null)
                    {
                        var hash = tx.GetSignatureHash(output.ScriptPubKey, i, hashType, output.Value);
                        var signature = secret.Sign(hash, hashType);

                        tx.Inputs[i].ScriptSig = PayToPubkeyHashTemplate.Instance.GenerateScriptSig(signature, secret.PubKey);
                        continue;
                    }
                    throw new BusinessException("Incompatible private key", ErrorCode.IncompatiblePrivateKey);
                }

                if (PayToPubkeyTemplate.Instance.CheckScriptPubKey(output.ScriptPubKey))
                {
                    var secret = GetPrivateKey(PayToPubkeyTemplate.Instance.ExtractScriptPubKeyParameters(output.ScriptPubKey).Hash);
                    if (secret != null)
                    {
                        var hash = tx.GetSignatureHash(output.ScriptPubKey, i, hashType, output.Value);
                        var signature = secret.Sign(hash, hashType);

                        tx.Inputs[i].ScriptSig = PayToPubkeyTemplate.Instance.GenerateScriptSig(signature);
                        continue;
                    }
                    throw new BusinessException("Incompatible private key", ErrorCode.InvalidScript);
                }

                if (PayToScriptHashTemplate.Instance.CheckScriptPubKey(output.ScriptPubKey))
                {
                    var secret = GetPrivateKey(PayToScriptHashTemplate.Instance.ExtractScriptPubKeyParameters(output.ScriptPubKey));

                    if (secret != null && secret.PubKey.WitHash.ScriptPubKey.Hash.ScriptPubKey == output.ScriptPubKey)
                    {
                        var hash = tx.GetSignatureHash(secret.PubKey.WitHash.AsKeyId().ScriptPubKey, i, hashType, output.Value, HashVersion.Witness);
                        var signature = secret.Sign(hash, hashType);
                        tx.Inputs[i].WitScript = PayToPubkeyHashTemplate.Instance.GenerateScriptSig(signature, secret.PubKey);
                        tx.Inputs[i].ScriptSig = new Script(Op.GetPushOp(secret.PubKey.WitHash.ScriptPubKey.ToBytes(true)));

                        continue;
                    }
                };


                throw new BusinessException("Incompatible private key", ErrorCode.InvalidScript);
            }

            return SignResult.Ok(tx.ToHex());
        }

        private Transaction CreateForkTransaction(Transaction transaction)
        {
            var tx = BGold.BitcoinGoldConsensusFactory.Instance.CreateTransaction();
            tx.Inputs.AddRange(transaction.Inputs);
            tx.Outputs.AddRange(transaction.Outputs);
            tx.LockTime = transaction.LockTime;
            tx.Version = transaction.Version;
            return tx;
        }
    }
}
