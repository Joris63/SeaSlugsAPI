using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SeaSlugAPI.Models;

namespace SeaSlugAPI.Controllers
{
    [Route("api/sea-slugs")]
    [ApiController]
    public class SeaSlugController : ControllerBase
    {
        

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] AddSeaSlugRequest model)
        {
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get(string id)
        {
            if(!Guid.TryParse(id, out var seaSlugId))
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAll()
        {
            return Ok();
        }

        [HttpPost]
        [Route("rename")]
        public async Task<IActionResult> Rename([FromBody] RenameSeaSlugRequest model)
        {
            return Ok();
        }

        [HttpPost]
        [Route("reorder")]
        public async Task<IActionResult> ReorderLabels([FromBody] ReorderSeaSlugsRequest model)
        {
            return Ok();
        }
    }
}
