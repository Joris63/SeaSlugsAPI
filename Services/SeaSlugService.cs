using Microsoft.EntityFrameworkCore;
using SeaSlugAPI.Entities;
using SeaSlugAPI.Helpers;
using SeaSlugAPI.Models;

namespace SeaSlugAPI.Services
{
    public interface ISeaSlugService
    {
        Task<BaseResponse> Add(AddSeaSlugRequest model);
        Task<BaseResponse> Edit(EditSeaSlugRequest model);
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

        public async Task<BaseResponse> Add(AddSeaSlugRequest model)
        {
            if (await LabelExists(model.Label))
            {
                return new BaseResponse() { Message = "Label already exists." };
            }

            try
            {
                SeaSlug newSlug = new SeaSlug() { Id = Guid.NewGuid(), Label = model.Label, Name = model.Name };

                await _context.SeaSlugs.AddAsync(newSlug);
                await _context.SaveChangesAsync();

                return new AddSeaSlugResponse() { Success = true, Message = "Sea Slug species added.", Label = newSlug.Label, Name = newSlug.Name };
            }
            catch (Exception ex)
            {
                return new BaseResponse() { Message = ex.Message };
            }
        }

        public async Task<BaseResponse> Edit(EditSeaSlugRequest model)
        {
            try
            {
                SeaSlug? retrievedSlug = await _context.SeaSlugs.FirstOrDefaultAsync(x => x.Id.Equals(model.Label));

                if (retrievedSlug == null)
                {
                    return new BaseResponse() { Message = "No sea slug found." };
                }

                retrievedSlug.Name = model.NewName;
                await _context.SaveChangesAsync();

                return new EditSeaSlugResponse() { Success = true, Message = "Sea slug name updated.", Label = retrievedSlug.Label, Name = retrievedSlug.Name };
            }
            catch(Exception ex)
            {
                return new BaseResponse() { Message = ex.Message };
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
