using Microsoft.AspNetCore.Mvc;
using SeaSlugAPI.Helpers;
using SeaSlugAPI.Models;
using SeaSlugAPI.Services;

namespace SeaSlugAPI.Controllers
{
    [Route("api/prediction")]
    [ApiController]
    public class PredictionController : ControllerBase
    {
        private readonly IAzureService _azureService;

        public PredictionController(IAzureService azureService)
        {
            _azureService = azureService;
        }

        [HttpPost]
        [Route("single")]
        [ProducesResponseType(typeof(SinglePredictionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PredictSingle([FromForm] SinglePredictionRequest model)
        {
            if (!ImageHelper.isValidImageFile(model.Image))
            {
                return BadRequest("Invalid image file type. Only JPEG and PNG are allowed.");
            }

            try
            {
                BaseResponse response = await _azureService.GetPrediction(model.Image);

                if (response is SinglePredictionResponse predictionResponse)
                {
                    return Ok(predictionResponse);
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
        [Route("batch")]
        public async Task<IActionResult> PredictBatch([FromForm] BatchPredictionRequest model)
        {
            return Ok("Nothing happened");
        }

        [HttpPost]
        [Route("validate")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ValidatePrediction([FromForm] ValidatePredictionRequest model)
        {
            if (!ImageHelper.isValidImageFile(model.Image))
            {
                return BadRequest("Invalid image file type. Only JPEG and PNG are allowed.");
            }

            try
            {
                BaseResponse response = await _azureService.UploadBlob(model);

                if(response.Success)
                {
                    return Ok(response.Message);
                }
                else
                {
                    return BadRequest(response.Message);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}