using Microsoft.AspNetCore.Mvc;
using SeaSlugAPI.Helpers;
using SeaSlugAPI.Models;
using SeaSlugAPI.Services;

namespace SeaSlugAPI.Controllers
{
    [Route("api/model")]
    [ApiController]
    public class ModelManagementController : ControllerBase
    {
        private readonly ISeaSlugService _seaSlugService;

        public ModelManagementController(ISeaSlugService seaSlugService)
        {
            _seaSlugService = seaSlugService;
        }

        [HttpGet]
        [Route("training-status")]
        public async Task<IActionResult> GetTrainingStatus()
        {
            return Ok("Nothing happened");
        }

        [HttpPost]
        [Route("add-label")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddLabel([FromBody] AddSeaSlugRequest model)
        {
            try
            {
                SeaSlugResponse response = await _seaSlugService.Add(model);

                if (response.Success)
                {
                    return Ok(response.Message);
                }
                else
                {
                    return BadRequest(response.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Route("change-label")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeLabelName([FromBody] EditSeaSlugRequest model)
        {
            try
            {
                SeaSlugResponse response = await _seaSlugService.Edit(model);

                if (response.Success)
                {
                    return Ok(response.Message);
                }
                else
                {
                    return BadRequest(response.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}