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

namespace ProxyAPI.Controllers
{
    /// <summary>
    /// Provides API REST-based Web services for Magic the Gathering Card Proxies.
    /// </summary>
    [Authorize]
    [AuthenticationFilter]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/{apiVersion:apiVersion}/[controller]")]
    public class CardController : ControllerBase
    {
        #region Members.
        private readonly int _trackingId;
        private readonly ProxyConfigs _configs;
        private readonly WizardsConsumer _consumer;
        private readonly ILogger<CardController> _logger;
        #endregion

        #region Contructors.
        public CardController(ProxyConfigs configs, IMagicConsumer consumer, ILogger<CardController> logger)
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
        /// Get the Magic the Gathering card details for the most recent printed version.
        /// </summary>
        /// <returns>CardDetails</returns>
        /// <example>GET: /api/1.0/card/newest/Liliana%20of%20the%20Veil</example>
        /// <response code="200">Success</response>
        /// <response code="400">Unable to get CardDetails due to validation errors.</response>
        /// <response code="500">Unable to get CardDetails due to processing errors.</response>
        [HttpGet("newest/{name:minlength(1)}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CardDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetCardNewestVersion([Required] string name)
        {
            EventId eventId = new EventId(_trackingId, "GetCardNewestVersion()");
            _logger.Log(LogLevel.Information, eventId, "Processing");

            if (!ModelState.IsValid)
            {
                string errMsg = "Status 400 Bad Request: Invalid Request.";
                _logger.Log(LogLevel.Warning, eventId, errMsg);
                return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: errMsg);
            }

            CardDetails result = null;

            try
            {
                List<MagicCardDTO> cardVersions = _consumer.GetCards(name);
                MagicCardDTO dto = (cardVersions == null) ? null : cardVersions.FirstOrDefault();
                if (dto == null)
                {
                    string errMsg = $"Status 400 Bad Request: Invalid Card Name={name}.";
                    _logger.Log(LogLevel.Warning, eventId, errMsg);
                    return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: errMsg);
                }

                // Map the private external DataTransferObject (DTO) to public API Model.
                result = new CardDetails()
                {
                    Name = dto.name,
                    ManaCost = dto.manaCost,
                    CMC = dto.cmc,
                    Colors = (dto.colors == null) ? null : dto.colors.ToList<string>(),
                    ColorIdentity = (dto.colorIdentity == null) ? null : dto.colorIdentity.ToList<string>(),
                    Type = dto.type,
                    Supertypes = (dto.supertypes == null) ? null : dto.supertypes.ToList<string>(),
                    Types = (dto.types == null) ? null : dto.types.ToList<string>(),
                    Subtypes = (dto.subtypes == null) ? null : dto.subtypes.ToList<string>(),
                    Rarity = dto.rarity,
                    Set = dto.set,
                    SetName = dto.setName,
                    Text = dto.text,
                    Flavor = dto.flavor,
                    Artist = dto.artist,
                    Number = dto.number,
                    Power = dto.power,
                    Toughness = dto.toughness,
                    Layout = dto.layout,
                    Watermark = dto.watermark,
                    Rulings = (dto.rulings == null) ? null : dto.rulings.Select(x => $"Date:{x.date},Text:{x.text}").ToList<string>(),
                    Printings = (dto.printings == null) ? null : dto.printings.ToList<string>(),
                    OriginalText = dto.originalText,
                    OriginalType = dto.originalType,
                    LegalFormats = (dto.legalities == null) ? null : dto.legalities.Select(x => $"Format:{x.format},Legality:{x.legality}").ToList<string>()
                };
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, eventId, ex, "Status 500 Internal Server Error. Unhandled Exception.");
                return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.Message);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get the Magic the Gathering card details for the oldest printed version.
        /// </summary>
        /// <returns>CardDetails</returns>
        /// <example>GET: /api/1.0/card/oldest/Liliana%20of%20the%20Veil</example>
        /// <response code="200">Success</response>
        /// <response code="400">Unable to get CardDetails due to validation errors.</response>
        /// <response code="500">Unable to get CardDetails due to processing errors.</response>
        [HttpGet("oldest/{name:minlength(1)}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CardDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetCardOldestVersion([Required] string name)
        {
            EventId eventId = new EventId(_trackingId, "GetCardOldestVersion()");
            _logger.Log(LogLevel.Information, eventId, "Processing");

            if (!ModelState.IsValid)
            {
                string errMsg = "Status 400 Bad Request: Invalid Request.";
                _logger.Log(LogLevel.Warning, eventId, errMsg);
                return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: errMsg);
            }

            CardDetails result = null;

            try
            {
                List<MagicCardDTO> cardVersions = _consumer.GetCards(name);
                MagicCardDTO dto = (cardVersions == null) ? null : cardVersions.LastOrDefault();
                if (dto == null)
                {
                    string errMsg = $"Status 400 Bad Request: Invalid Card Name={name}.";
                    _logger.Log(LogLevel.Warning, eventId, errMsg);
                    return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: errMsg);
                }

                // Map the private external DataTransferObject (DTO) to public API Model.
                result = new CardDetails()
                {
                    Name = dto.name,
                    ManaCost = dto.manaCost,
                    CMC = dto.cmc,
                    Colors = (dto.colors == null) ? null : dto.colors.ToList<string>(),
                    ColorIdentity = (dto.colorIdentity == null) ? null : dto.colorIdentity.ToList<string>(),
                    Type = dto.type,
                    Supertypes = (dto.supertypes == null) ? null : dto.supertypes.ToList<string>(),
                    Types = (dto.types == null) ? null : dto.types.ToList<string>(),
                    Subtypes = (dto.subtypes == null) ? null : dto.subtypes.ToList<string>(),
                    Rarity = dto.rarity,
                    Set = dto.set,
                    SetName = dto.setName,
                    Text = dto.text,
                    Flavor = dto.flavor,
                    Artist = dto.artist,
                    Number = dto.number,
                    Power = dto.power,
                    Toughness = dto.toughness,
                    Layout = dto.layout,
                    Watermark = dto.watermark,
                    Rulings = (dto.rulings == null) ? null : dto.rulings.Select(x => $"Date:{x.date},Text:{x.text}").ToList<string>(),
                    Printings = (dto.printings == null) ? null : dto.printings.ToList<string>(),
                    OriginalText = dto.originalText,
                    OriginalType = dto.originalType,
                    LegalFormats = (dto.legalities == null) ? null : dto.legalities.Select(x => $"Format:{x.format},Legality:{x.legality}").ToList<string>()
                };
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, eventId, ex, "Status 500 Internal Server Error. Unhandled Exception.");
                return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.Message);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get the Magic the Gathering card details for all printed versions of the card.
        /// </summary>
        /// <returns>List CardDetails</returns>
        /// <example>GET: /api/1.0/card/Liliana%20of%20the%20Veil</example>
        /// <response code="200">Success</response>
        /// <response code="400">Unable to get CardDetails due to validation errors.</response>
        /// <response code="500">Unable to get CardDetails due to processing errors.</response>
        [HttpGet("{name:minlength(1)}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CardDetails>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetCardVersions([Required] string name)
        {
            EventId eventId = new EventId(_trackingId, "GetCardVersions()");
            _logger.Log(LogLevel.Information, eventId, "Processing");

            if (!ModelState.IsValid)
            {
                string errMsg = "Status 400 Bad Request: Invalid Request.";
                _logger.Log(LogLevel.Warning, eventId, errMsg);
                return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: errMsg);
            }

            List<CardDetails> result = null;

            try
            {
                List<MagicCardDTO> cardVersions = _consumer.GetCards(name);
                if (cardVersions == null || cardVersions.Count <= 0)
                {
                    string errMsg = $"Status 400 Bad Request: Invalid Card Name={name}.";
                    _logger.Log(LogLevel.Warning, eventId, errMsg);
                    return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: errMsg);
                }

                result = new List<CardDetails>();
                foreach (MagicCardDTO dto in cardVersions)
                {
                    // Map the private external DataTransferObject (DTO) to public API Model.
                    result.Add(new CardDetails()
                    {
                        Name = dto.name,
                        ManaCost = dto.manaCost,
                        CMC = dto.cmc,
                        Colors = (dto.colors == null) ? null : dto.colors.ToList<string>(),
                        ColorIdentity = (dto.colorIdentity == null) ? null : dto.colorIdentity.ToList<string>(),
                        Type = dto.type,
                        Supertypes = (dto.supertypes == null) ? null : dto.supertypes.ToList<string>(),
                        Types = (dto.types == null) ? null : dto.types.ToList<string>(),
                        Subtypes = (dto.subtypes == null) ? null : dto.subtypes.ToList<string>(),
                        Rarity = dto.rarity,
                        Set = dto.set,
                        SetName = dto.setName,
                        Text = dto.text,
                        Flavor = dto.flavor,
                        Artist = dto.artist,
                        Number = dto.number,
                        Power = dto.power,
                        Toughness = dto.toughness,
                        Layout = dto.layout,
                        Watermark = dto.watermark,
                        Rulings = (dto.rulings == null) ? null : dto.rulings.Select(x => $"Date:{x.date},Text:{x.text}").ToList<string>(),
                        Printings = (dto.printings == null) ? null : dto.printings.ToList<string>(),
                        OriginalText = dto.originalText,
                        OriginalType = dto.originalType,
                        LegalFormats = (dto.legalities == null) ? null : dto.legalities.Select(x => $"Format:{x.format},Legality:{x.legality}").ToList<string>()
                    });
                }
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
