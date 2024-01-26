using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SeaSlugAPI.Authentication;
using SeaSlugAPI.Entities;
using SeaSlugAPI.Entities.DTOs;
using SeaSlugAPI.Models;
using SeaSlugAPI.Services;

namespace SeaSlugAPI.Controllers
{
    [Route("api/training")]
    [ApiController]
    [ServiceFilter(typeof(ApiKeyAuthFilter))]
    public class ModelTrainingController : ControllerBase
    {
        private readonly IBlobStorageService _blobStorageService;
        private readonly ITrainingLogService _trainingLogService;
        private readonly IAzureService _azureService;

        public ModelTrainingController(IBlobStorageService blobStorageService, ITrainingLogService trainingLogService, IAzureService azureService)
        {
            _blobStorageService = blobStorageService;
            _trainingLogService = trainingLogService;
            _azureService = azureService;
        }

        /// <summary>
        /// Retrieves the newest log of the AI model training.
        /// </summary>
        [HttpGet]
        [Route("log")]
        [ProducesResponseType(typeof(TrainingLogDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTrainingLog()
        {
            try
            {
                // Get latest training log
                TrainingLogServiceResults results = await _trainingLogService.GetLatest();

                // Check if it succeeded
                if (results.Success)
                {
                    // Check Data
                    if (results.Data != null)
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
        /// Retrieves a log of the AI model training using an Id
        /// </summary>
        [HttpGet]
        [Route("log/{id}")]
        [ProducesResponseType(typeof(TrainingLogDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTrainingLog(string id)
        {
            // Convert the string id to a Guid
            if (!Guid.TryParse(id, out var trainingLogId))
            {
                return BadRequest("Id has to be a valid Guid.");
            }

            try
            {
                // Get training log by id
                TrainingLogServiceResults results = await _trainingLogService.GetById(trainingLogId);

                // Check if it succeeded
                if (results.Success)
                {
                    // Check Data
                    if (results.Data != null)
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
        /// Creates a new log of the AI model training.
        /// </summary>
        [HttpPost]
        [Route("log")]
        [ProducesResponseType(typeof(TrainingLogDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateTrainingLog()
        {
            try
            {
                // Add training log to the DB
                TrainingLogServiceResults results = await _trainingLogService.Add();

                // Check if it succeeded
                if (results.Success)
                {
                    return Ok(results.Data);
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
        /// Edits a log of the AI model training.
        /// </summary>
        [HttpPut]
        [Route("log")]
        [ProducesResponseType(typeof(TrainingLogDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EditTraingLog(EditTrainingLogRequest model)
        {
            try
            {
                // Edit training log
                TrainingLogServiceResults results = await _trainingLogService.Edit(model);

                // Check if it succeeded
                if (results.Success)
                {
                    // Check Data
                    if (results.Data != null)
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
        /// Starts the training of the AI model using the validated data.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> StartTraining(bool fromScratch)
        {
            try
            {
                // Start the training process
                TrainingResponse results = await _azureService.RequestModelTraining(fromScratch);

                // Check if it succeeded
                if (results.Success)
                {
                    return Ok(results.Message);
                }
                else
                {
                    return StatusCode(500, results.Message);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return StatusCode(500, "An unexpected error occurred on the server. Please try again later.");
            }
        }

        /// <summary>
        /// Retrieves the data count for each species that the AI will train on.
        /// </summary>
        [HttpGet]
        [Route("data-count")]
        [ProducesResponseType(typeof(List<ValidatedDataCount>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
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
        /// Retrieves the validated data files to further train the current AI model.
        /// </summary>
        [HttpGet]
        [Route("retraining-files")]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRetrainingDataFiles()
        {
            try
            {
                // Get the training data count per sea slug
                BlobStorageResponse<List<string>> results = await _blobStorageService.RetrieveRetrainingData();

                // Check if it succeeded
                if (results.Success && results.Data != null)
                {
                    return Ok(results.Data);
                }
                else
                {
                    return StatusCode(500, results.Message);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return StatusCode(500, "An unexpected error occurred on the server. Please try again later.");
            }
        }

        /// <summary>
        /// Retrieves the data files for each species to retrain the AI model from scratch.
        /// </summary>
        [HttpGet]
        [Route("training-files")]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTrainingDataFiles()
        {
            try
            {
                // Get the training data count per sea slug
                BlobStorageResponse<List<string>> results = await _blobStorageService.RetrieveTrainingData();

                // Check if it succeeded
                if (results.Success && results.Data != null)
                {
                    return Ok(results.Data);
                }
                else
                {
                    return StatusCode(500, results.Message);
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
