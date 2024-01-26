using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SeaSlugAPI.Authentication;
using SeaSlugAPI.Entities.DTOs;
using SeaSlugAPI.Helpers;
using SeaSlugAPI.Models;
using SeaSlugAPI.Services;

namespace SeaSlugAPI.Controllers
{
    [Route("api/sea-slugs")]
    [ApiController]
    [ServiceFilter(typeof(ApiKeyAuthFilter))]
    public class SeaSlugController : ControllerBase
    {
        private readonly ISeaSlugService _seaSlugService;

        public SeaSlugController(ISeaSlugService seaSlugService)
        {
            _seaSlugService = seaSlugService;
        }

        /// <summary>
        /// Adds a new sea slug species to the DB.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(SeaSlugDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Add([FromBody] AddSeaSlugRequest model)
        {
            try
            {
                // Add the new sea slug to the DB
                SeaSlugServiceResults<SeaSlugDTO> results = await _seaSlugService.Add(model);

                // Check if it succeeded
                if (results.Success)
                {
                    // Check Data
                    if (results.Data != null)
                    {
                        return Ok(results.Data);
                    }
                    else
                    {
                        return BadRequest(results.Message);
                    }
                }
                else
                {
                    return StatusCode(500, "An unexpected error occurred on the server. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return StatusCode(500, "An unexpected error occurred on the server. Please try again later.");
            }
        }

        /// <summary>
        /// Retrieves a sea slug species by their Guid ID.
        /// </summary>
        [HttpGet("byId/{id}")]
        [ProducesResponseType(typeof(SeaSlugDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(string id)
        {
            // Convert the string id to a Guid
            if (!Guid.TryParse(id, out var seaSlugId))
            {
                return BadRequest("Id has to be a valid Guid.");
            }

            try
            {
                // Get the sea slug by Id
                SeaSlugServiceResults<SeaSlugDTO> results = await _seaSlugService.GetById(seaSlugId);

                // Check if it succeeded
                if (results.Success)
                {
                    // Check Data
                    if (results.Data != null)
                    {
                        return Ok(results.Data);
                    }
                    else
                    {
                        return NotFound(results.Message);
                    }
                }
                else
                {
                    return StatusCode(500, "An unexpected error occurred on the server. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return StatusCode(500, "An unexpected error occurred on the server. Please try again later.");
            }
        }

        /// <summary>
        /// Retrieves a sea slug species by their label number.
        /// </summary>
        [HttpGet("byLabel/{label}")]
        [ProducesResponseType(typeof(SeaSlugDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByLabel(string label)
        {
            // Convert the string label to an int
            if (!int.TryParse(label, out var seaSlugLabel))
            {
                return BadRequest("Label has to come in the form of an integer and has to be higher than 0.");
            }

            if (seaSlugLabel < 1)
            {
                return BadRequest("Label to be an integer higher than 0.");
            }

            try
            {
                // Get the sea slug
                SeaSlugServiceResults<SeaSlugDTO> results = await _seaSlugService.GetByLabel(seaSlugLabel);

                // Check if it succeeded
                if (results.Success)
                {
                    // Check Data
                    if (results.Data != null)
                    {
                        return Ok(results.Data);
                    }
                    else
                    {
                        return NotFound(results.Message);
                    }
                }
                else
                {
                    return StatusCode(500, "An unexpected error occurred on the server. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return StatusCode(500, "An unexpected error occurred on the server. Please try again later.");
            }
        }

        /// <summary>
        /// Retrieves all the sea slug species.
        /// </summary>
        [HttpGet]
        [Route("all")]
        [ProducesResponseType(typeof(List<SeaSlugDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                // Get the sea slug
                SeaSlugServiceResults<List<SeaSlugDTO>> results = await _seaSlugService.GetAll();

                // Check if it succeeded
                if (results.Success)
                {
                    // Check Data
                    if (results.Data?.Count > 0)
                    {
                        return Ok(results.Data);
                    }
                    else
                    {
                        return NotFound(results.Message);
                    }
                }
                else
                {
                    return StatusCode(500, "An unexpected error occurred on the server. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return StatusCode(500, "An unexpected error occurred on the server. Please try again later.");
            }
        }

        /// <summary>
        /// Renames a sea slug species.
        /// </summary>
        [HttpPatch]
        [Route("rename")]
        [ProducesResponseType(typeof(SeaSlugDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Rename([FromBody] RenameSeaSlugRequest model)
        {
            try
            {
                // Rename the sea slug
                SeaSlugServiceResults<SeaSlugDTO> results = await _seaSlugService.Rename(model);

                // Check if it succeeded
                if (results.Success)
                {
                    // Check Data
                    if (results.Data != null)
                    {
                        return Ok(results.Data);
                    }
                    else
                    {
                        return BadRequest(results.Message);
                    }
                }
                else
                {
                    return StatusCode(500, "An unexpected error occurred on the server. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return StatusCode(500, "An unexpected error occurred on the server. Please try again later.");
            }
        }

        /// <summary>
        /// Reorders all the sea slug labels to match that of the AI model.
        /// </summary>
        [HttpPut]
        [Route("reorder")]
        [ProducesResponseType(typeof(List<SeaSlugDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ReorderLabels([FromBody] ReorderSeaSlugsRequest model)
        {
            try
            {
                // Reorder all the sea slug labels
                SeaSlugServiceResults<List<SeaSlugDTO>> results = await _seaSlugService.ReorderLabels(model);

                // Check if it succeeded
                if (results.Success)
                {
                    // Check Data
                    if (results.Data?.Count > 0)
                    {
                        return Ok(results.Data);
                    }
                    else
                    {
                        return BadRequest(results.Message);
                    }
                }
                else
                {
                    return StatusCode(500, "An unexpected error occurred on the server. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return StatusCode(500, "An unexpected error occurred on the server. Please try again later.");
            }
        }
    }
}
