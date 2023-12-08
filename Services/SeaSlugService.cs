using Microsoft.EntityFrameworkCore;
using SeaSlugAPI.Context;
using SeaSlugAPI.Entities;
using SeaSlugAPI.Entities.DTOs;
using SeaSlugAPI.Helpers;
using SeaSlugAPI.Models;

namespace SeaSlugAPI.Services
{
    public interface ISeaSlugService
    {
        Task<SeaSlugServiceResults<SeaSlugDTO>> Add(AddSeaSlugRequest model);
        Task<SeaSlugServiceResults<SeaSlugDTO>> GetById(Guid id);
        Task<SeaSlugServiceResults<SeaSlugDTO>> GetByLabel(int label);
        Task<SeaSlugServiceResults<List<SeaSlugDTO>>> GetAll();
        Task<SeaSlugServiceResults<SeaSlugDTO>> Rename(RenameSeaSlugRequest model);
        Task<SeaSlugServiceResults<List<SeaSlugDTO>>> ReorderLabels(ReorderSeaSlugsRequest model);
    }

    public class SeaSlugService : ISeaSlugService
    {
        private readonly AppDbContext _dbContext;

        public SeaSlugService(AppDbContext context)
        {
            _dbContext = context;
        }

        public async Task<SeaSlugServiceResults<SeaSlugDTO>> Add(AddSeaSlugRequest model)
        {
            // Create a new sea slug entity
            SeaSlug newSlug = new SeaSlug(model.Name);

            try
            {
                // Check the highest label number currently listed in the DB
                SeaSlug? highestLabelSlug = await _dbContext.SeaSlugs
                    .OrderByDescending(x => x.Label)
                    .FirstOrDefaultAsync();

                // If there is no entry found then use label 1
                newSlug.Label = (highestLabelSlug?.Label ?? 0) + 1;

                // Add to DB and save changes
                await _dbContext.SeaSlugs.AddAsync(newSlug);
                await _dbContext.SaveChangesAsync();

                return new SeaSlugServiceResults<SeaSlugDTO>(DTOConverter.SeaSlugToDTO(newSlug), true, "Successfully added sea slug.");
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return new SeaSlugServiceResults<SeaSlugDTO>("Could not add sea slug.");
            }
        }

        public async Task<SeaSlugServiceResults<SeaSlugDTO>> GetById(Guid id)
        {
            try
            {
                // Retrieve sea slug using Guid id
                SeaSlug? slugFound = await _dbContext.SeaSlugs.FirstOrDefaultAsync(x => x.Id == id);

                // Check if it found it
                if(slugFound == null)
                {
                    return new SeaSlugServiceResults<SeaSlugDTO>(true, "Could not find sea slug.");
                }

                return new SeaSlugServiceResults<SeaSlugDTO>(DTOConverter.SeaSlugToDTO(slugFound), true, "Successfully found sea slug.");
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return new SeaSlugServiceResults<SeaSlugDTO>("Could not find sea slug.");
            }
        }

        public async Task<SeaSlugServiceResults<SeaSlugDTO>> GetByLabel(int label)
        {
            try
            {
                // Retrieve sea slug using int label
                SeaSlug? slugFound = await _dbContext.SeaSlugs.FirstOrDefaultAsync(x => x.Label == label);

                // Check if it found it
                if (slugFound == null)
                {
                    return new SeaSlugServiceResults<SeaSlugDTO>(true, "Could not find sea slug.");
                }

                return new SeaSlugServiceResults<SeaSlugDTO>(DTOConverter.SeaSlugToDTO(slugFound), true, "Successfully found sea slug.");
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return new SeaSlugServiceResults<SeaSlugDTO>("Could not find sea slug.");
            }
        }

        public async Task<SeaSlugServiceResults<List<SeaSlugDTO>>> GetAll()
        {
            try
            {
                // Retrieve all the sea slugs
                List<SeaSlug> seaSlugs = await _dbContext.SeaSlugs.ToListAsync() ?? new List<SeaSlug>();

                // Check if it found any
                if (seaSlugs.Count < 1)
                {
                    return new SeaSlugServiceResults<List<SeaSlugDTO>> (true, "Could not find any sea slugs.");
                }

                // Convert the entities to dto's
                List<SeaSlugDTO> seaSlugDTOs = new List<SeaSlugDTO>();
                foreach(SeaSlug slug in seaSlugs) 
                {
                    seaSlugDTOs.Add(DTOConverter.SeaSlugToDTO(slug));
                }

                return new SeaSlugServiceResults<List<SeaSlugDTO>>(seaSlugDTOs, true, "Successfully found any sea slugs.");
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return new SeaSlugServiceResults<List<SeaSlugDTO>>("Could not find any sea slugs.");
            }
        }

        public async Task<SeaSlugServiceResults<SeaSlugDTO>> Rename(RenameSeaSlugRequest model)
        {
            try
            {
                // Retrieve sea slug using Guid id
                SeaSlug? seaSlug = await _dbContext.SeaSlugs.FirstOrDefaultAsync(x => x.Id == model.Id);

                // Check if it found anything
                if(seaSlug == null)
                {
                    return new SeaSlugServiceResults<SeaSlugDTO>(true, "Could not find sea slug.");
                }

                // Rename the sea slug
                seaSlug.Name = model.NewName;

                // Save changes to DB
                await _dbContext.SaveChangesAsync();

                return new SeaSlugServiceResults<SeaSlugDTO>(DTOConverter.SeaSlugToDTO(seaSlug), true, "Successfully renamed sea slug.");
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return new SeaSlugServiceResults<SeaSlugDTO>("Could not find sea slug.");
            }
        }

        public async Task<SeaSlugServiceResults<List<SeaSlugDTO>>> ReorderLabels(ReorderSeaSlugsRequest model)
        {
            try
            {
                // Retrieve all the sea slugs
                List<SeaSlug> seaSlugs = await _dbContext.SeaSlugs.ToListAsync();

                // Go through each sea slug found and change the label of it
                foreach (SeaSlug seaSlug in seaSlugs)
                {
                    int newLabel = model.OrderedSeaSlugIds.FindIndex(x => x == seaSlug.Id);

                    // Check if the id was found in the list
                    // if not cancel the process
                    if (newLabel != -1)
                    {
                        seaSlug.Label = newLabel;
                    }
                    else
                    {
                        return new SeaSlugServiceResults<List<SeaSlugDTO>>(true, "Unable to reorder labels. Requires new labels for all the slugs.");
                    }
                }

                // Cancel the process if there is a duplicate label
                if (HasDuplicateLabels(seaSlugs))
                {
                    return new SeaSlugServiceResults<List<SeaSlugDTO>>(true, "Unable to reorder labels. Duplicate labels were found.");
                }

                // Save changes in DB
                await _dbContext.SaveChangesAsync();

                // Convert the entities to dto's
                List<SeaSlugDTO> seaSlugDTOs = new List<SeaSlugDTO>();
                foreach (SeaSlug slug in seaSlugs)
                {
                    seaSlugDTOs.Add(DTOConverter.SeaSlugToDTO(slug));
                }

                return new SeaSlugServiceResults<List<SeaSlugDTO>>(seaSlugDTOs, true, "Successfully reordered the labels.");
            }
            catch(Exception ex)
            {
                Console.Write(ex.Message);
                return new SeaSlugServiceResults<List<SeaSlugDTO>>("Unable to reorder labels.");
            }
        }

        private bool HasDuplicateLabels(List<SeaSlug> seaSlugs)
        {
            var seenValues = new HashSet<int>();
            var duplicates = new HashSet<int>();

            foreach (SeaSlug slug in seaSlugs)
            {
                int label = slug.Label;

                if (!seenValues.Add(label))
                {
                    // If the value has already been seen, it's a duplicate
                    duplicates.Add(label);
                }
            }

            // If duplicates set is not empty, there are duplicates
            return duplicates.Count > 0;
        }
    }
}