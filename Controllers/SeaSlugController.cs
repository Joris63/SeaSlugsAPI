using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SeaSlugAPI.Entities.DTOs;
using SeaSlugAPI.Helpers;
using SeaSlugAPI.Models;
using SeaSlugAPI.Services;

namespace SeaSlugAPI.Controllers
{
    [Route("api/sea-slugs")]
    [ApiController]
    public class SeaSlugController : ControllerBase
    {
        private readonly ISeaSlugService _seaSlugService;

        public SeaSlugController(ISeaSlugService seaSlugService)
        {
            _seaSlugService = seaSlugService;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddSeaSlugRequest model)
        {
            try
            {
                // Add the new sea slug to the DB and create a container for it in the blob storage
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

        [HttpGet("byId/{id}")]
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
                        return NotFound();
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

        [HttpGet("byLabel/{label}")]
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
                        return NotFound();
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

        [HttpGet]
        [Route("all")]
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
                        return NotFound();
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

        [HttpPost]
        [Route("rename")]
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

        [HttpPost]
        [Route("reorder")]
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
                    if (results.Data.Count > 0)
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
