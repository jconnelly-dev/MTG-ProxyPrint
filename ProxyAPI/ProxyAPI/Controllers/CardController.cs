using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProxyAPI.Security;
using ProxyAPI.Models;
using MagicConsumer;
using MagicConsumer.WizardsAPI;
using MagicConsumer.WizardsAPI.DTO;
using System.Threading.Tasks;
using System.IO;

namespace ProxyAPI.Controllers
{
    /// <summary>
    /// Provides Magic the Gathering card information.
    /// </summary>
    [AuthenticationFilter]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/{apiVersion:apiVersion}/[controller]")]
    public class CardController : ControllerBase
    {
        #region Members.
        private readonly int _trackingId;
        private readonly AppOptions _configs;
        private readonly WizardsConsumer _consumer;
        private readonly ILogger<CardController> _logger;
        #endregion

        #region Contructors.
        public CardController(ILogger<CardController> logger, IMagicConsumer consumer, AppOptions configs)
        {
            // Every request to this controller will have a sudo unique (random) trackingId.
            Random rand = new Random();
            _trackingId = rand.Next(0, int.MaxValue);

            _logger = logger;
            _configs = configs;
            _consumer = (WizardsConsumer)consumer;
        }
        #endregion

        #region Web API Endpoints - Card Information.
        /// <summary>
        /// Get the Magic the Gathering card detail for the most recent printed version of named card.
        /// </summary>
        /// <returns>CardDetail</returns>
        /// <example>GET: /api/1.0/card/info/newest/name=angel's grace</example>
        /// <response code="200">Success</response>
        /// <response code="400">Unable to get CardDetail due to validation errors.</response>
        /// <response code="500">Unable to get CardDetail due to processing errors.</response>
        [HttpGet("info/newest/{name:minlength(1)}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CardDetail))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetCardInfoNewestVersion([Required][FromRoute] string name)
        {
            EventId eventId = new EventId(_trackingId, "GetCardInfoNewestVersion()");
            _logger.Log(LogLevel.Information, eventId, "Processing");

            if (!ModelState.IsValid)
            {
                string errMsg = "Status 400 Bad Request: Invalid Request.";
                _logger.Log(LogLevel.Warning, eventId, errMsg);
                return Problem(statusCode: StatusCodes.Status400BadRequest, detail: errMsg);
            }

            CardDetail result = null;

            try
            {
                List<MagicCardDTO> cardVersions = null;

                try
                {
                    cardVersions = _consumer.GetCards(name);
                }
                catch (Exception ex)
                {
                    string errMsg = $"Status 500 Internal Server Error. {ex.Message}.";
                    _logger.Log(LogLevel.Error, eventId, errMsg);
                    return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: errMsg);
                }

                MagicCardDTO dto = (cardVersions == null) ? null : cardVersions.FirstOrDefault();
                if (dto == null)
                {
                    string errMsg = $"Status 400 Bad Request: Invalid Card Name={name}.";
                    _logger.Log(LogLevel.Warning, eventId, errMsg);
                    return Problem(statusCode: StatusCodes.Status400BadRequest, detail: errMsg);
                }

                result = MapInternalToExternal(dto);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, eventId, ex, "Status 500 Internal Server Error. Unhandled Exception.");
                return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.Message);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get the Magic the Gathering card detail for the oldest printed version of named card.
        /// </summary>
        /// <returns>CardDetail</returns>
        /// <example>GET: /api/1.0/card/info/oldest/name=liliana%20of%20the%20veil</example>
        /// <response code="200">Success</response>
        /// <response code="400">Unable to get CardDetail due to validation errors.</response>
        /// <response code="500">Unable to get CardDetail due to processing errors.</response>
        [HttpGet("info/oldest/{name:minlength(1)}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CardDetail))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetCardInfoOldestVersion([Required][FromRoute] string name)
        {
            EventId eventId = new EventId(_trackingId, "GetCardInfoOldestVersion()");
            _logger.Log(LogLevel.Information, eventId, "Processing");

            if (!ModelState.IsValid)
            {
                string errMsg = "Status 400 Bad Request: Invalid Request.";
                _logger.Log(LogLevel.Warning, eventId, errMsg);
                return Problem(statusCode: StatusCodes.Status400BadRequest, detail: errMsg);
            }

            CardDetail result = null;

            try
            {
                List<MagicCardDTO> cardVersions = null;

                try
                {
                    cardVersions = _consumer.GetCards(name);
                }
                catch (Exception ex)
                {
                    string errMsg = $"Status 500 Internal Server Error. {ex.Message}.";
                    _logger.Log(LogLevel.Error, eventId, errMsg);
                    return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: errMsg);
                }

                MagicCardDTO dto = (cardVersions == null) ? null : cardVersions.LastOrDefault();
                if (dto == null)
                {
                    string errMsg = $"Status 400 Bad Request: Invalid Card Name={name}.";
                    _logger.Log(LogLevel.Warning, eventId, errMsg);
                    return Problem(statusCode: StatusCodes.Status400BadRequest, detail: errMsg);
                }

                result = MapInternalToExternal(dto);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, eventId, ex, "Status 500 Internal Server Error. Unhandled Exception.");
                return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.Message);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get the Magic the Gathering card detail for all printed versions of the named card.
        /// </summary>
        /// <returns>List CardDetail</returns>
        /// <example>GET: /api/1.0/card/info/name=fury</example>
        /// <response code="200">Success</response>
        /// <response code="400">Unable to get CardDetails due to validation errors.</response>
        /// <response code="500">Unable to get CardDetails due to processing errors.</response>
        [HttpGet("info/{name:minlength(1)}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CardDetail>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetCardVersions([Required][FromRoute] string name)
        {
            EventId eventId = new EventId(_trackingId, "GetCardVersions()");
            _logger.Log(LogLevel.Information, eventId, "Processing");

            if (!ModelState.IsValid)
            {
                string errMsg = "Status 400 Bad Request. Invalid Request.";
                _logger.Log(LogLevel.Warning, eventId, errMsg);
                return Problem(statusCode: StatusCodes.Status400BadRequest, detail: errMsg);
            }

            List<CardDetail> result = null;

            try
            {
                List<MagicCardDTO> cardVersions = null;

                try
                {
                    cardVersions = _consumer.GetCards(name);
                }
                catch (Exception ex)
                {
                    string errMsg = $"Status 500 Internal Server Error. {ex.Message}.";
                    _logger.Log(LogLevel.Error, eventId, errMsg);
                    return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: errMsg);
                }

                if (cardVersions == null || cardVersions.Count <= 0)
                {
                    string errMsg = $"Status 400 Bad Request. Invalid Card Name={name}.";
                    _logger.Log(LogLevel.Warning, eventId, errMsg);
                    return Problem(statusCode: StatusCodes.Status400BadRequest, detail: errMsg);
                }

                result = new List<CardDetail>();
                foreach (MagicCardDTO dto in cardVersions)
                {
                    CardDetail details = MapInternalToExternal(dto);
                    if (details != null)
                    {
                        result.Add(details);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, eventId, ex, "Status 500 Internal Server Error. Unhandled Exception.");
                return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.Message);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get the Magic the Gathering card detail for all printed versions of the named cards.
        /// </summary>
        /// <returns>List CardDetail</returns>
        /// <example>GET: /api/1.0/card/info/names=island,mountain</example>
        /// <response code="200">Success</response>
        /// <response code="400">Unable to get CardDetails due to validation errors.</response>
        /// <response code="500">Unable to get CardDetails due to processing errors.</response>
        [HttpGet("info/{names}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CardDetail>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetCardsVersions([Required][FromRoute] List<string> names)
        {
            EventId eventId = new EventId(_trackingId, "GetCardsVersions()");
            _logger.Log(LogLevel.Information, eventId, "Processing");

            if (!ModelState.IsValid)
            {
                string errMsg = "Status 400 Bad Request. Invalid Request.";
                _logger.Log(LogLevel.Warning, eventId, errMsg);
                return Problem(statusCode: StatusCodes.Status400BadRequest, detail: errMsg);
            }

            List<CardDetail> result = null;

            try
            {
                List<MagicCardDTO> cardsVersions = null;
                string namesString = string.Join(",", names);

                try
                {
                    Parallel.ForEach(names, name =>
                    {
                        if (!string.IsNullOrEmpty(name) && !string.IsNullOrWhiteSpace(name))
                        {
                            List<MagicCardDTO> cardVersions = _consumer.GetCards(name);
                            if (cardVersions != null && cardsVersions.Count > 0)
                            {
                                lock (cardsVersions)
                                {
                                    cardsVersions.AddRange(cardVersions);
                                }
                            }
                        }
                    });              
                }
                catch (Exception ex)
                {
                    string errMsg = $"Status 500 Internal Server Error. {ex.Message}.";
                    _logger.Log(LogLevel.Error, eventId, errMsg);
                    return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: errMsg);
                }

                if (cardsVersions == null || cardsVersions.Count <= 0)
                {
                    string errMsg = $"Status 400 Bad Request. Invalid Card Names={namesString}.";
                    _logger.Log(LogLevel.Warning, eventId, errMsg);
                    return Problem(statusCode: StatusCodes.Status400BadRequest, detail: errMsg);
                }

                result = new List<CardDetail>();
                foreach (MagicCardDTO dto in cardsVersions)
                {
                    CardDetail details = MapInternalToExternal(dto);
                    if (details != null)
                    {
                        result.Add(details);
                    }
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

        #region Web API Endpoints - Card Images.
        /// <summary>
        /// Get the Magic the Gathering image for the most recent printed version of named card.
        /// </summary>
        /// <returns>CardImage</returns>
        /// <example>GET: /api/1.0/card/image/newest/name=angel's grace</example>
        /// <response code="200">Success</response>
        /// <response code="400">Unable to get CardImage due to validation errors.</response>
        /// <response code="500">Unable to get CardImage due to processing errors.</response>
        [HttpGet("image/newest/{name:minlength(1)}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(File))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetCardImageNewestVersion([Required][FromRoute] string name)
        {
            EventId eventId = new EventId(_trackingId, "GetCardImageNewestVersion()");
            _logger.Log(LogLevel.Information, eventId, "Processing");

            if (!ModelState.IsValid)
            {
                string errMsg = "Status 400 Bad Request: Invalid Request.";
                _logger.Log(LogLevel.Warning, eventId, errMsg);
                return Problem(statusCode: StatusCodes.Status400BadRequest, detail: errMsg);
            }

            string contentType = null;
            byte[] binaryImage = null;

            try
            {
                List<MagicCardDTO> cardVersions = _consumer.GetCards(name);
                List<MagicCardDTO> cardImageVersions = (cardVersions == null) ? null : cardVersions.Where(x => x.multiverseid != null && x.imageUrl != null).OrderBy(x => x.multiverseid).ToList<MagicCardDTO>();
                if (cardImageVersions != null && cardImageVersions.Count > 0)
                {
                    MagicCardDTO newestVersion = cardImageVersions.FirstOrDefault();
                    if (newestVersion != null && newestVersion.multiverseid != null && newestVersion.imageUrl != null)
                    {
                        string imagePath = _consumer.DownloadCardImage(newestVersion, _configs.DownloadPath);
                        if (imagePath != null)
                        {
                            binaryImage = System.IO.File.ReadAllBytes(imagePath);
                            contentType = "image/png";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, eventId, ex, "Status 500 Internal Server Error. Unhandled Exception.");
                return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.Message);
            }

            if (binaryImage == null || binaryImage.Length <= 0 || string.IsNullOrEmpty(contentType))
            {
                string errMsg = "Status 500 Internal Server Error. Failed to create binary image.";
                _logger.Log(LogLevel.Warning, eventId, errMsg);
                return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: errMsg);
            }

            return File(binaryImage, contentType);
        }

        /// <summary>
        /// Get the Magic the Gathering image for the oldest printed version of named card.
        /// </summary>
        /// <returns>CardImage</returns>
        /// <example>GET: /api/1.0/card/image/oldest/name=liliana%20of%20the%20veil</example>
        /// <response code="200">Success</response>
        /// <response code="400">Unable to get CardImage due to validation errors.</response>
        /// <response code="500">Unable to get CardImage due to processing errors.</response>
        [HttpGet("image/oldest/{name:minlength(1)}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(File))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetCardImageOldestVersion([Required][FromRoute] string name)
        {
            EventId eventId = new EventId(_trackingId, "GetCardImageOldestVersion()");
            _logger.Log(LogLevel.Information, eventId, "Processing");

            if (!ModelState.IsValid)
            {
                string errMsg = "Status 400 Bad Request: Invalid Request.";
                _logger.Log(LogLevel.Warning, eventId, errMsg);
                return Problem(statusCode: StatusCodes.Status400BadRequest, detail: errMsg);
            }

            string contentType = null;
            byte[] binaryImage = null;

            try
            {
                List<MagicCardDTO> cardVersions = _consumer.GetCards(name);
                List<MagicCardDTO> cardImageVersions = (cardVersions == null) ? null : cardVersions.Where(x => x.multiverseid != null && x.imageUrl != null).OrderByDescending(x => x.multiverseid).ToList<MagicCardDTO>();
                if (cardImageVersions != null && cardImageVersions.Count > 0)
                {
                    MagicCardDTO oldestVersion = cardImageVersions.FirstOrDefault();
                    if (oldestVersion != null && oldestVersion.multiverseid != null && oldestVersion.imageUrl != null)
                    {
                        string imagePath = _consumer.DownloadCardImage(oldestVersion, _configs.DownloadPath);
                        if (imagePath != null)
                        {
                            binaryImage = System.IO.File.ReadAllBytes(imagePath);
                            contentType = "image/png";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, eventId, ex, "Status 500 Internal Server Error. Unhandled Exception.");
                return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.Message);
            }

            if (binaryImage == null || binaryImage.Length <= 0 || string.IsNullOrEmpty(contentType))
            {
                string errMsg = "Status 500 Internal Server Error. Failed to create binary image.";
                _logger.Log(LogLevel.Warning, eventId, errMsg);
                return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: errMsg);
            }

            return File(binaryImage, contentType);
        }
        #endregion

        #region Shared Controller Methods.
        private static CardDetail MapInternalToExternal(MagicCardDTO dto)
        {
            CardDetail result = null;
            if (dto != null)
            {
                result = new CardDetail()
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
            return result;
        }
        #endregion
    }
}
