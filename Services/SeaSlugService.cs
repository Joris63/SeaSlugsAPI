using Microsoft.EntityFrameworkCore;
using SeaSlugAPI.Entities;
using SeaSlugAPI.Helpers;
using SeaSlugAPI.Models;

namespace SeaSlugAPI.Services
{
    public interface ISeaSlugService
    {
        Task<SeaSlugResponse> Add(AddSeaSlugRequest model);
        Task<SeaSlugResponse> Edit(EditSeaSlugRequest model);
    }

    public class SeaSlugService : ISeaSlugService
    {
        private readonly AppDbContext _context;
        private readonly IAzureService _azureService;

        public SeaSlugService(AppDbContext context, IAzureService azureService)
        {
            _context = context;
            _azureService = azureService;
        }

        public async Task<SeaSlugResponse> Add(AddSeaSlugRequest model)
        {
            if (await LabelExists(model.Label))
            {
                return new SeaSlugResponse() { Message = "Label already exists." };
            }

            try
            {
                SeaSlug newSlug = new SeaSlug() { Id = Guid.NewGuid(), Label = model.Label, Name = model.Name };

                await _context.SeaSlugs.AddAsync(newSlug);

                BlobStorageResponse containerCreateResponse = await _azureService.CreateContainer(newSlug.Id.ToString());

                if (containerCreateResponse.Success)
                {
                    await _context.SaveChangesAsync();
                }

                return new SeaSlugResponse() { Success = true, Message = "Sea Slug species added.", SeaSlug = DTOConverter.SeaSlugToDTO(newSlug) };
            }
            catch (Exception ex)
            {
                return new SeaSlugResponse() { Message = ex.Message };
            }
        }

        public async Task<SeaSlugResponse> Edit(EditSeaSlugRequest model)
        {
            try
            {
                SeaSlug? retrievedSlug = await _context.SeaSlugs.FirstOrDefaultAsync(x => x.Id.Equals(model.Id));

                if (retrievedSlug == null)
                {
                    return new SeaSlugResponse() { Message = "No sea slug found." };
                }

                if(model.Label != -1)
                {
                    retrievedSlug.Label = model.Label;
                }

                if (model.Name != string.Empty)
                {
                    retrievedSlug.Name = model.Name;
                }

                await _context.SaveChangesAsync();

                return new SeaSlugResponse() { Success = true, Message = "Sea slug name updated.", SeaSlug = DTOConverter.SeaSlugToDTO(retrievedSlug) };
            }
            catch (Exception ex)
            {
                return new SeaSlugResponse() { Message = ex.Message };
            }
        }

        public async Task<bool> LabelExists(int label)
        {
            if (await _context.SeaSlugs.AnyAsync(x => x.Label == label))
            {
                return true;
            }

            return false;
        }
    }
}
