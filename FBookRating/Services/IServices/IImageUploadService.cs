namespace FBookRating.Services.IServices
{
    public interface IImageUploadService
    {
        Task<string> UploadImageAsync(IFormFile imageFile);
    }
}
