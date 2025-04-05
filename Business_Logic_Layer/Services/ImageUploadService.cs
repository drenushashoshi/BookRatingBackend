using FBookRating.Services.IServices;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace FBookRating.Services
{
    public class ImageUploadService : IImageUploadService
    {
        private readonly HttpClient _httpClient;
        private readonly string _imgurClientId;

        public ImageUploadService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _imgurClientId = "ed18bdc292f6dae"; // Replace with your Imgur Client ID
        }

        public async Task<string> UploadImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("Invalid image file.");

            using (var content = new MultipartFormDataContent())
            using (var imageStream = new MemoryStream())
            {
                await imageFile.CopyToAsync(imageStream);
                imageStream.Seek(0, SeekOrigin.Begin);

                var imageContent = new StreamContent(imageStream);
                imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

                content.Add(imageContent, "image", imageFile.FileName);

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Client-ID", _imgurClientId);
                var response = await _httpClient.PostAsync("https://api.imgur.com/3/upload", content);

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Failed to upload image to Imgur.");

                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JObject.Parse(responseJson);
                return responseData["data"]["link"].ToString(); // Return the Imgur URL
            }
        }
    }
}
