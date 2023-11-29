using Microsoft.AspNetCore.Mvc;
using SeaSlugAPI.Helpers;
using SeaSlugAPI.Models;
using SeaSlugAPI.Services;

namespace SeaSlugAPI.Controllers
{
    [Route("api/predictions")]
    [ApiController]
    public class PredictionController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Predict([FromForm] PredictionRequest model)
        {
            return Ok();
        }

        [HttpPost]
        [Route("batch")]
        public async Task<IActionResult> PredictBatch([FromForm] BatchPredictionRequest model)
        {
            return Ok();
        }

        [HttpPost]
        [Route("validate")]
        public async Task<IActionResult> ValidatePrediction([FromForm] ValidatePredictionRequest model)
        {
            return Ok();
        }
    }
}