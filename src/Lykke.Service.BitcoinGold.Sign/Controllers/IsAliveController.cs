﻿using System.Linq;
using System.Net;
using Lykke.Common.Api.Contract.Responses;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using IsAliveResponse = Lykke.Service.BitcoinGold.Sign.Models.IsAliveResponse;

namespace Lykke.Service.BitcoinGold.Sign.Controllers
{
    // NOTE: See https://lykkex.atlassian.net/wiki/spaces/LKEWALLET/pages/35755585/Add+your+app+to+Monitoring
    [Route("api/[controller]")]
    public class IsAliveController : Controller
    {

        /// <summary>
        /// Checks service is alive
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SwaggerOperation("IsAlive")]
        [ProducesResponseType(typeof(IsAliveResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public IActionResult Get()
        {
            // NOTE: Feel free to extend IsAliveResponse, to display job-specific indicators
            return Ok(new IsAliveResponse
            {
                Name = Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationName,
                Version = Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationVersion,
                Env = Program.EnvInfo,
#if DEBUG
                IsDebug = true,
#else
                IsDebug = false,
#endif
                IssueIndicators = Enumerable.Empty<IsAliveResponse.IssueIndicator>()
            });
        }
    }
}
