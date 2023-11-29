using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SeaSlugAPI.Controllers
{
    [Route("api/training")]
    [ApiController]
    public class ModelTrainingController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetStatus()
        {
            return Ok();
        }
    }
}
