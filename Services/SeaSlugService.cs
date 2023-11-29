using Microsoft.EntityFrameworkCore;
using SeaSlugAPI.Entities;
using SeaSlugAPI.Entities.DTOs;
using SeaSlugAPI.Helpers;
using SeaSlugAPI.Models;

namespace SeaSlugAPI.Services
{
    public interface ISeaSlugService
    {
        Task<SeaSlugDTO> Add(AddSeaSlugRequest model);
        Task<SeaSlugDTO> Get(Guid id);
        Task<List<SeaSlugDTO>> GetAll();
        Task<SeaSlugDTO> Rename(RenameSeaSlugRequest model);
        Task<List<SeaSlugDTO>> ReorderLabels(ReorderSeaSlugsRequest model);
    }

    public class SeaSlugService : ISeaSlugService
    {
        public async Task<SeaSlugDTO> Add(AddSeaSlugRequest model)
        {
            return new SeaSlugDTO();
        }

        public async Task<SeaSlugDTO> Get(Guid id)
        {
            return new SeaSlugDTO();
        }

        public async Task<List<SeaSlugDTO>> GetAll()
        {
            return new List<SeaSlugDTO>();
        }

        public async Task<SeaSlugDTO> Rename(RenameSeaSlugRequest model)
        {
            return new SeaSlugDTO();
        }

        public async Task<List<SeaSlugDTO>> ReorderLabels(ReorderSeaSlugsRequest model)
        {
            return new List<SeaSlugDTO>();
        }
    }
}