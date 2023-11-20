using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SeaSlugAPI.Helpers;
using SeaSlugAPI.Models;
using SeaSlugAPI.Services;

namespace SeaSlugAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeaSlugsController : ControllerBase
    {
        private readonly IAzureService _azureService;

        public SeaSlugsController(IAzureService azureService)
        {
            _azureService = azureService;
        }

        [HttpPost]
        [Route("predict")]
        public async Task<IActionResult> Predict([FromForm] PredictModel model)
        {
            try
            {
                string base64String = ImageToBase64Converter.ConvertImageToBase64(model.Image);

                PredictionResponse response = await _azureService.GetPrediction(base64String);

                if (response.Probabilities.Count > 0)
                {
                    return Ok(response);
                }
                else
                {
                    return NotFound(response);
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