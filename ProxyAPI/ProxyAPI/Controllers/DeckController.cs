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
using ProxyAPI.Security;
using ProxyAPI.DTO;
using ProxyAPI.Models;
using MagicConsumer;
using MagicConsumer.WizardsAPI;
using MagicConsumer.WizardsAPI.DTO;
using System.IO;
using System.Text;
using ProxyAPI.Parsers;

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
        [FileUploadSecurityValidation()]
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
                string uploadDeckPath = null;

                // Create a new/unique directory for this request that will store the decklist text file and all downloaded card images.
                int currAttempt = 1;
                int maxAttempts = 5;
                bool uploadPathExists = false;
                do
                {
                    guid = Guid.NewGuid().ToString();
                    uploadDeckPath = Path.Combine(_configs.InputPath, guid);
                    if (!Directory.Exists(uploadDeckPath))
                    {
                        Directory.CreateDirectory(uploadDeckPath);
                        if (Directory.Exists(uploadDeckPath))
                        {
                            uploadPathExists = true;
                            break;
                        }
                    }
                    currAttempt++;
                }
                while (currAttempt <= maxAttempts);
                if (!uploadPathExists || string.IsNullOrEmpty(guid) || string.IsNullOrEmpty(uploadDeckPath))
                {
                    string errMsg = $"Status 500 Internal Server Error. Unable to Find Upload Pathing.";
                    _logger.Log(LogLevel.Error, eventId, errMsg);
                    return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: errMsg);
                }

                // Open request body file, parse, validate, synchronously copy to new text file, then return valid cotent in a SimpleDeck.
                string decklistFilePath = Path.Combine(uploadDeckPath, DECKLIST_FILENAME);
                SimpleDeck parsedDeck = ProcessPlainTextDecklistFile(decklist, decklistFilePath, guid);

                // Determine if file was uploaded and parsed successfully.
                if (parsedDeck == null || parsedDeck.Cards == null || parsedDeck.Cards.Count <= 0 || !System.IO.File.Exists(decklistFilePath))
                {
                    string errMsg = $"Status 500 Internal Server Error. Failed to Parse Decklist.";
                    _logger.Log(LogLevel.Error, eventId, errMsg);
                    return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: errMsg);
                }

                // Download each card image found in the parsed deck list.
                foreach (SimpleCard card in parsedDeck.Cards)
                {
                    List<MagicCardDTO> cardVersions = _consumer.GetCards(card.Name);
                    foreach (MagicCardDTO cardVersion in cardVersions)
                    {
                        if (cardVersion != null)
                        {
                            string downloadImagePath = _consumer.DownloadCardImage(cardVersion, uploadDeckPath);
                            if (!string.IsNullOrEmpty(downloadImagePath))
                            {
                                // blah...
                            }
                        }
                    }
                }
                
                //IProxyParser parser = new ProxyTextFileParser(decklistFilePath);
                //List<SimpleDeck> simpleDecks = parser.CollectDeckLists();
                //if (simpleDecks == null || simpleDecks.Count <= 0)
                //{
                //    string errMsg = $"Status 400 Bad Request. Unable to Parse Uploaded Decklist.";
                //    _logger.Log(LogLevel.Warning, eventId, errMsg);
                //    return Problem(statusCode: StatusCodes.Status400BadRequest, detail: errMsg);
                //}

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

        #region Shared Controller Methods.
        private static void SyncCopyFullPlainTextFileToFile(IFormFile decklist, string decklistFilePath)
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

        private SimpleDeck ProcessPlainTextDecklistFile(IFormFile decklist, string decklistFilePath, string guid)
        {
            SimpleDeck result = new SimpleDeck(guid);

            string line = null;
            using (Stream stream = decklist.OpenReadStream())
            using (StreamReader reader = new StreamReader(stream))
            using (StreamWriter writer = new StreamWriter(decklistFilePath))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    string validLine;
                    SimpleCard card = ProxyTextFileParser.ParseLine(line, out validLine);
                    if (card != null && validLine != null)
                    {
                        result.Cards.Add(card);
                        writer.WriteLine(validLine);
                    }
                }
            }
            if (result.Cards == null || result.Cards.Count <= 0)
            {
                result = null;
            }

            return result;
        }
        #endregion
    }
}
