using System;

namespace Lykke.Service.BitcoinGold.Sign.Core.Exceptions
{
    public class BusinessException : Exception
    {
        public ErrorCode Code { get; }
        public string Text { get; }

        public BusinessException(string text, ErrorCode code)
            : base(text)
        {
            Code = code;
            Text = text;
        }
    }

    public enum ErrorCode
    {
        IncompatiblePrivateKey,

        InvalidScript,
        InputNotFound

    }
}
