using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using SeaSlugAPI.Helpers;
using SeaSlugAPI.Models;
using SeaSlugAPI.Services;

namespace SeaSlugAPI.Controllers
{
    [Route("api/predictions")]
    [ApiController]
    public class PredictionController : ControllerBase
    {
        private readonly IAzureMLService _azureMLService;

        public PredictionController(IAzureMLService azureMLService)
        {
            _azureMLService = azureMLService;
        }

        [HttpPost]
        public async Task<IActionResult> Predict([FromForm] PredictionRequest model)
        {
            try
            {
                // Check if the image is of type png or jpeg
                if (!ImageHelper.isValidImageFile(model.Image))
                {
                    return BadRequest("Invalid image file type. Only JPEG and PNG are allowed.");
                }

                // Request prediction from the model in Azure ML Studio
                PredictionResults results = await _azureMLService.Predict(model);

                // Check if it succeeded
                if(results.Success)
                {
                    // Check for the probabilities count
                    if(results.Probabilities.Count > 0)
                    {
                        return Ok(results.Probabilities);
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