using System.Net;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BitcoinGold.Sign.Core.Sign;
using Lykke.Service.BitcoinGold.Sign.Extensions;
using Lykke.Service.BitcoinGold.Sign.Models.Sign;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.BitcoinGold.Sign.Controllers
{
    [Route("api/[controller]")]
    public class SignController : Controller
    {
        private readonly ITransactionSigningService _transactionSigningService;

        public SignController(ITransactionSigningService transactionSigningService)
        {
            _transactionSigningService = transactionSigningService;
        }

        [HttpPost]
        [SwaggerOperation(nameof(SignRawTx))]
        [ProducesResponseType(typeof(SignOkTransactionResponce), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public IActionResult SignRawTx([FromBody]SignRequest sourceTx)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorResponse.Create("ValidationError").AddModelStateErrors(ModelState));
            }

            var signResult = _transactionSigningService.Sign(sourceTx.TransactionContext, sourceTx.PrivateKeys);

            var respResult = new SignOkTransactionResponce
            {
                SignedTransaction = signResult.TransactionHex
            };

            return Ok(respResult);
        }
    }
}
