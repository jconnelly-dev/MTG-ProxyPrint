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
using System.Text;

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
        const string DECKLIST_FILENAME = "decklist.txt";

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
        /// Upload text file containing deck list.
        /// </summary>
        /// <returns>UploadIdentifier</returns>
        /// <format>(card_quantity)(white_space)(card_name)</format>
        /// <format_ex1>4 Ponder</format_ex1>
        /// <format_ex2>2 Lightning Bolt</format_ex2>
        /// <format_ex3>10 Swamp</format_ex3>
        /// <example>POST: /api/1.0/deck</example>
        /// <response code="200">Success</response>
        /// <response code="400">Unable to get Upload File due to processing errors.</response>
        /// <response code="500">Unable to get Upload File due to validation errors.</response>
        [HttpPost()]
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

            // NOTE: I'll make this all into an Attribute later when I go through clean-up process.
            // MIME Sniffing standards should be used here to enforce a bunch of security concerns w/reading in a file from the request.
            string[] allowedFileTypes = new string[]
            {
                "text/plain"
            };
            if (decklist.Length <= 0 || decklist.ContentType == null || !allowedFileTypes.Contains(decklist.ContentType))
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

                // Create a new/unique directory for this request that will store the decklist text file and all downloaded card images.
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
                    string errMsg = $"Status 500 Internal Server Error. Unable to Find Upload Decklist Pathing.";
                    _logger.Log(LogLevel.Error, eventId, errMsg);
                    return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: errMsg);
                }

                // Inside new request directory, create a file and synchronously copy to it the plain text contents from the request body file.
                string decklistFilePath = Path.Combine(decklistPath, DECKLIST_FILENAME);
                bool validCopy = SyncCopyValidPlainTextFileToFile(decklist, decklistFilePath);

                // Determine if file was uploaded successfully.
                if (!validCopy || !System.IO.File.Exists(decklistFilePath))
                {
                    string errMsg = $"Status 500 Internal Server Error. Failed to Upload Decklist Text File.";
                    _logger.Log(LogLevel.Error, eventId, errMsg);
                    return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: errMsg);
                }

                result = new UploadIdentifier(guid);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, eventId, ex, "Status 500 Internal Server Error. Unhandled Exception.");
                return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.Message);
            }

            return Ok(result);
        }
        #endregion


        private void SyncCopyFullPlainTextFileToFile(IFormFile decklist, string decklistFilePath)
        {
            /*
             * NOTE: this function is no longer used, but was the original strategy used by this API.
             *  I am keeping a copy of it mostly for reference, although I do not suggest using this strategy for safety concerns.
             *  This is the most basic way to copy a plain text file from the request body to a new file created based on the specified pathing.
             */

            // Create a file, then copy the file contents from the request body IFormFile to the server synchronously.
            using (FileStream fileStream = new FileStream(decklistFilePath, FileMode.Create))
            {
                decklist.CopyTo(fileStream);
            }
        }

        private bool SyncCopyValidPlainTextFileToFile(IFormFile decklist, string decklistFilePath)
        {
            bool valid = false;

            string line = null;
            using (Stream stream = decklist.OpenReadStream())
            using (StreamReader reader = new StreamReader(stream))
            using (StreamWriter writer = new StreamWriter(decklistFilePath))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(line) && !string.IsNullOrWhiteSpace(line))
                    {
                        string trimmedLine = line.Trim();

                        // Check to see if the first character of the current plain text line is a digit.
                        if (!string.IsNullOrWhiteSpace(trimmedLine) && char.IsDigit(trimmedLine[0]))
                        {
                            writer.WriteLine(trimmedLine);
                            valid = true;
                        }
                    }
                }
            }

            return valid;
        }
    }
}
