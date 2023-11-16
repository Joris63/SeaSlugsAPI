namespace SeaSlugAPI.Helpers
{
    public class ImageToBase64Converter
    {
        public static string ConvertImageToBase64(IFormFile image)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                image.CopyTo(memoryStream);
                byte[] imageBytes = memoryStream.ToArray();

                // Convert the byte array to a Base64 string
                return Convert.ToBase64String(imageBytes);
            }
        }
    }
}
