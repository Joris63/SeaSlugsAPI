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
        /// Retrieves the newest log of the AI model training.
        /// </summary>
        [HttpGet]
        [Route("log")]
        public async Task<IActionResult> GetTrainingLog()
        {
            return Ok();
        }

        /// <summary>
        /// Retrieves a log of the AI model training using an Id
        /// </summary>
        [HttpGet]
        [Route("log/{id}")]
        public async Task<IActionResult> GetTrainingLog(string id)
        {
            return Ok();
        }

        /// <summary>
        /// Creates a new log of the AI model training.
        /// </summary>
        [HttpPost]
        [Route("log")]
        public async Task<IActionResult> CreateTrainingLog()
        {
            return Ok();
        }

        /// <summary>
        /// Edits a log of the AI model training.
        /// </summary>
        [HttpPut]
        [Route("log")]
        public async Task<IActionResult> EditTraingLog()
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
        [Route("data-count")]
        public async Task<IActionResult> GetTrainingDataCount()
        {
            try
            {
                // Get the training data count per sea slug
                BlobStorageResponse<List<ValidatedDataCount>> results = await _blobStorageService.RetrieveTrainingDataCount();

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
        [Route("data-files")]
        public async Task<IActionResult> GetTrainingDataFiles()
        {
            try
            {
                // Get the training data count per sea slug
                BlobStorageResponse<Stream> results = await _blobStorageService.RetrieveTrainingData();

                // Check if it succeeded
                if (results.Success && results.Data != null)
                {
                    // Set appropriate response headers
                    Response.Headers.Append("Content-Encoding", "gzip");
                    Response.Headers.Append("Content-Type", "image.jpeg");

                    // Return the ZIP stream using FileStreamResult
                    return new FileStreamResult(results.Data, "image/jpeg");
                }
                else
                {
                    return BadRequest(results.Message);
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
