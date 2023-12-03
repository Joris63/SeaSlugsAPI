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

        [HttpGet]
        public async Task<IActionResult> GetStatus()
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> StartTraining()
        {
            return Ok();
        }

        [HttpGet]
        [Route("training-data")]
        public async Task<IActionResult> RetrieveTrainingData()
        {
            return Ok();
        }
    }
}
