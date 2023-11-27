using System.Reflection.Emit;

namespace SeaSlugAPI.Models
{
    public class AddSeaSlugResponse : BaseResponse
    {
        public int Label { get; set; }
        public string Name { get; set; }
    }
}
