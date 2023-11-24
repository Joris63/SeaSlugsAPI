namespace SeaSlugAPI.Helpers
{
    public class ImageHelper
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

        public static bool isValidImageFile(IFormFile file)
        {
            if (file == null || file.Length == 0) 
            {
                return false;
            }

            var contentType = file.ContentType;

            // Compare against a whitelist of allowed content types
            var allowedContentTypes = new[] { "image/jpeg", "image/jpg", "image/png" };

            return allowedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase);
        }
    }
}
