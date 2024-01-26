using Microsoft.EntityFrameworkCore;
using SeaSlugAPI.Entities.DTOs;
using SeaSlugAPI.Entities;
using SeaSlugAPI.Helpers;
using SeaSlugAPI.Models;
using SeaSlugAPI.Context;

namespace SeaSlugAPI.Services
{
    public interface ITrainingLogService
    {
        Task<TrainingLogServiceResults> Add();
        Task<TrainingLogServiceResults> GetLatest();
        Task<TrainingLogServiceResults> GetById(Guid id);
        Task<TrainingLogServiceResults> Edit(EditTrainingLogRequest model);
    }

    public class TrainingLogService : ITrainingLogService
    {
        private readonly AppDbContext _dbContext;

        public TrainingLogService(AppDbContext context)
        {
            _dbContext = context;
        }

        public async Task<TrainingLogServiceResults> Add()
        {
            // Create a new training log entity
            TrainingLog log = new TrainingLog();

            try
            {
                // Add to DB and save changes
                await _dbContext.TrainingLogs.AddAsync(log);
                await _dbContext.SaveChangesAsync();

                return new TrainingLogServiceResults(DTOConverter.TrainingLogToDTO(log), true, "Successfully added training log.");
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return new TrainingLogServiceResults("Could not add training log.");
            }
        }

        public async Task<TrainingLogServiceResults> GetLatest()
        {
            try
            {
                // Retrieve training log using Guid id
                TrainingLog? log = await _dbContext.TrainingLogs.OrderByDescending(x => x.StartDate).FirstOrDefaultAsync(); ;

                // Check if it found it
                if (log == null)
                {
                    return new TrainingLogServiceResults(true, "Could not find training log.");
                }

                return new TrainingLogServiceResults(DTOConverter.TrainingLogToDTO(log), true, "Successfully found training log.");
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return new TrainingLogServiceResults("Could not find training log.");
            }
        }

        public async Task<TrainingLogServiceResults> GetById(Guid id)
        {
            try
            {
                // Retrieve training log using Guid id
                TrainingLog? log = await _dbContext.TrainingLogs.FirstOrDefaultAsync(x => x.Id == id);

                // Check if it found it
                if (log == null)
                {
                    return new TrainingLogServiceResults(true, "Could not find training log.");
                }

                return new TrainingLogServiceResults(DTOConverter.TrainingLogToDTO(log), true, "Successfully found training log.");
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return new TrainingLogServiceResults("Could not find training log.");
            }
        }

        public async Task<TrainingLogServiceResults> Edit(EditTrainingLogRequest model)
        {
            try
            {
                // Retrieve training log using Guid id
                TrainingLog? log = await _dbContext.TrainingLogs.FirstOrDefaultAsync(x => x.Id == model.Id);

                // Check if it found it
                if (log == null)
                {
                    return new TrainingLogServiceResults(true, "Could not find training log.");
                }

                // Edit training status
                log.Status = model.Status;

                // If training failed then add error
                if (model.Status == TrainingStatus.Failed)
                {
                    log.Error = model.Error;
                }

                // Save changes to DB
                await _dbContext.SaveChangesAsync();

                return new TrainingLogServiceResults(DTOConverter.TrainingLogToDTO(log), true, "Successfully updated training log.");
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return new TrainingLogServiceResults("Could not find training log.");
            }
        }
    }
}
