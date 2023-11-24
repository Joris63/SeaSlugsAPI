using Microsoft.AspNetCore.Mvc;
using SeaSlugAPI.Models;

namespace SeaSlugAPI.Controllers
{
    [Route("api/model")]
    [ApiController]
    public class ModelManagementController : ControllerBase
    {
        [HttpGet]
        [Route("training-status")]
        public async Task<IActionResult> GetTrainingStatus()
        {
            return Ok("Nothing happened");
        }

        [HttpPost]
        [Route("add-label")]
        public async Task<IActionResult> AddLabel([FromBody] AddLabelRequest model)
        {
            return Ok("Nothing happened");
        }

        [HttpPost]
        [Route("change-label")]
        public async Task<IActionResult> ChangeLabelName([FromBody] EditLabelRequest model)
        {
            return Ok("Nothing happened");
        }
    }
}