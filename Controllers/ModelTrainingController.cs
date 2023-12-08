using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SeaSlugAPI.Entities.DTOs;
using SeaSlugAPI.Models;
using SeaSlugAPI.Services;

namespace SeaSlugAPI.Controllers
{
    [Route("api/training")]
    [ApiController]
    public class ModelTrainingController : ControllerBase
    {
        private readonly IBlobStorageService _blobStorageService;

        public ModelTrainingController(IBlobStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }

        /// <summary>
        /// Retrieves the current status of the AI model training.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetStatus()
        {
            return Ok();
        }

        /// <summary>
        /// Starts the training of the AI model using the validated data.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> StartTraining()
        {
            return Ok();
        }

        /// <summary>
        /// Retrieves the data count for each species that the AI will train on.
        /// </summary>
        [HttpGet]
        [Route("training-data-count")]
        public async Task<IActionResult> GetTrainingDataCount()
        {
            try
            {
                // Get the training data count per sea slug
                BlobStorageResponse<List<SeaSlugValidatedDataCount>> results = await _blobStorageService.RetrieveTrainingDataCount();

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
        /// Retrieves the data files for each species that the AI will train on.
        /// </summary>
        [HttpGet]
        [Route("training-data-files")]
        public async Task<IActionResult> GetTrainingDataFiles()
        {
            return Ok();
        }
    }
}
