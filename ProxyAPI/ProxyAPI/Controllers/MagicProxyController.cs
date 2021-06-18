using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProxyAPI.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using ProxyAPI.DTO;
using ProxyAPI.Models;

namespace ProxyAPI.Controllers
{
    /// <summary>
    /// Provides API Web services for Magic the Gathering Proxies.
    /// </summary>
    [Authorize]
    [AuthenticationFilter]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/{apiVersion:apiVersion}/[controller]")]
    public class MagicProxyController : ControllerBase
    {
        #region Members.
        private readonly int _trackingId;
        private readonly ProxyConfigs _configs;
        private readonly ILogger<MagicProxyController> _logger;
        #endregion

        #region Contructors.
        public MagicProxyController(ProxyConfigs configs, ILogger<MagicProxyController> logger)
        {
            // Every request to this controller will have a sudo unique (random) trackingId.
            Random rand = new Random();
            _trackingId = rand.Next(0, int.MaxValue);

            _logger = logger;
            _configs = configs;
        }
        #endregion

        #region Web API Endpoints.
        /// <summary>
        /// Get the blah...
        /// </summary>
        /// <returns>blah...</returns>
        /// <example>GET: /api/1.0/card</example>
        /// <response code="200">Success</response>
        /// <response code="400">Unable to get blah... due to processing errors.</response>
        /// <response code="500">Unable to get blah... due to validation errors.</response>
        [HttpGet("card/{name:minlength(1)}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CardIdentifier))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetCard([Required] string name)
        {
            EventId eventId = new EventId(_trackingId, "GetCard()");
            _logger.Log(LogLevel.Information, eventId, "Processing");

            if (!ModelState.IsValid)
            {
                string errMsg = "Status 400 Bad Request: Invalid Request.";
                _logger.Log(LogLevel.Error, eventId, errMsg);
                return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: errMsg);
            }

            CardIdentifier result = null;

            try
            {
                result = new CardIdentifier();
                result.Name = name;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, eventId, ex, "Status 500 Internal Server Error. Unhandled Exception.");
                return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.Message);
            }

            return Ok(result);
        }
        #endregion
    }
}
