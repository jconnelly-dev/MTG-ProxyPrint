using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using ProxyAPI.Authentication;
using ProxyAPI.DTO;
using ProxyAPI.Models;
using MagicConsumer;

namespace ProxyAPI.Controllers
{
    /// <summary>
    /// Provides API REST-based Web services for Magic the Gathering Deck Proxies.
    /// </summary>
    [Authorize]
    [AuthenticationFilter]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/{apiVersion:apiVersion}/[controller]")]
    public class DeckController : ControllerBase
    {
        #region Members.
        private readonly int _trackingId;
        private readonly ProxyConfigs _configs;
        private readonly IMagicConsumer _consumer;
        private readonly ILogger<DeckController> _logger;
        #endregion

        #region Contructors.
        public DeckController(ProxyConfigs configs, IMagicConsumer consumer, ILogger<DeckController> logger)
        {
            // Every request to this controller will have a sudo unique (random) trackingId.
            Random rand = new Random();
            _trackingId = rand.Next(0, int.MaxValue);

            _logger = logger;
            _configs = configs;
            _consumer = consumer;
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CardDetails))]
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

            CardDetails result = null;

            try
            {
                result = new CardDetails();
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
