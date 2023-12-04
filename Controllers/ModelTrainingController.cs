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
            return Ok();
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
