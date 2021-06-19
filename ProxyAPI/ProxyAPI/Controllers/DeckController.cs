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
using MagicConsumer.WizardsAPI;
using MagicConsumer.WizardsAPI.DTO;
using System.IO;

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
        private readonly WizardsConsumer _consumer;
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
            _consumer = (WizardsConsumer)consumer;
        }
        #endregion

        #region Web API Endpoints.
        /// <summary>
        /// Upload file containing deck list. Valid file line's begin w/a numeric, followed by whitespace, followed by the card name. i.e. "4 Liliana of the Veil".
        /// </summary>
        /// <returns>UploadIdentifier</returns>
        /// <example>POST: /api/1.0/deck</example>
        /// <response code="200">Success</response>
        /// <response code="400">Unable to get Upload file due to processing errors.</response>
        /// <response code="500">Unable to get Upload file due to validation errors.</response>
        [HttpPost("card/{name:minlength(1)}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UploadIdentifier))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UploadDecklist([Required] IFormFile decklist)
        {
            EventId eventId = new EventId(_trackingId, "UploadDecklist()");
            _logger.Log(LogLevel.Information, eventId, "Processing");

            if (!ModelState.IsValid)
            {
                string errMsg = "Status 400 Bad Request: Invalid Request.";
                _logger.Log(LogLevel.Error, eventId, errMsg);
                return Problem(statusCode: StatusCodes.Status400BadRequest, detail: errMsg);
            }

            UploadIdentifier result = null;

            try
            {
                string guid = null;
                string decklistPath = null;

                int currAttempt = 1;
                int maxAttempts = 5;
                bool uploadPathExists = false;
                do
                {
                    guid = Guid.NewGuid().ToString();
                    decklistPath = Path.Combine(_configs.InputPath, guid);
                    if (!Directory.Exists(decklistPath))
                    {
                        Directory.CreateDirectory(decklistPath);
                        if (Directory.Exists(decklistPath))
                        {
                            uploadPathExists = true;
                            break;
                        }
                    }
                    currAttempt++;
                }
                while (currAttempt <= maxAttempts);
                if (!uploadPathExists || string.IsNullOrEmpty(guid) || string.IsNullOrEmpty(decklistPath))
                {
                    string errMsg = $"Status 500 Internal Server Error. Unable to Find Upload Pathing.";
                    _logger.Log(LogLevel.Error, eventId, errMsg);
                    return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: errMsg);
                }

                // Copy the file to the server synchronously, waiting for it to finish.
                string decklistFilePath = Path.Combine(decklistPath, guid);
                using (FileStream fileStream = new FileStream(decklistFilePath, FileMode.Create))
                {
                    decklist.CopyTo(fileStream);
                }

                // Determine if file was uploaded successfully.
                if (!System.IO.File.Exists(decklistFilePath))
                {
                    string errMsg = $"Status 500 Internal Server Error. Failed to Upload Decklist.";
                    _logger.Log(LogLevel.Error, eventId, errMsg);
                    return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: errMsg);
                }

                result = new UploadIdentifier();
                result.Identifier = guid;
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
