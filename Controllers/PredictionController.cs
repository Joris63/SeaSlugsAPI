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
        private readonly IBlobStorageService _blobStorageService;

        public PredictionController(IAzureMLService azureMLService, IBlobStorageService blobStorageService)
        {
            _azureMLService = azureMLService;
            _blobStorageService = blobStorageService;
        }

        /// <summary>
        /// Predicts the species of the sea slug image requested.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(List<SlugProbability>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
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
                if (results.Success)
                {
                    // Check for the probabilities count
                    if (results.Probabilities.Count > 0)
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

        /// <summary>
        /// Predicts the species of all the sea slug images requested.
        /// </summary>
        [HttpPost]
        [Route("batch")]
        public async Task<IActionResult> PredictBatch([FromForm] BatchPredictionRequest model)
        {
            return Ok();
        }

        /// <summary>
        /// Validates the sea slug prediction and saves the data for future AI training.
        /// </summary>
        [HttpPost]
        [Route("validate")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ValidatePrediction([FromForm] ValidatePredictionRequest model)
        {
            try
            {
                // Check if the image is of type png or jpeg
                if (!ImageHelper.isValidImageFileJPEG(model.Image))
                {
                    return BadRequest("Invalid image file type. Only JPEG allowed.");
                }

                // Request prediction from the model in Azure ML Studio
                BlobStorageResponse results = await _blobStorageService.UploadBlob(model);

                // Check if it succeeded
                if (results.Success)
                {
                    return Ok(results.Message);
                }
                else
                {
                    // Check if something went wrong internally
                    if (results.Error != string.Empty)
                    {
                        return StatusCode(500, "An unexpected error occurred on the server. Please try again later.");
                    }
                    else
                    {
                        return BadRequest(results.Message);
                    }
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