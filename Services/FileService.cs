using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace habyx.Services
{
    public class FileService
    {
        private readonly IWebHostEnvironment _environment;
        private const string ImageFolder = "ProfileImages";

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveProfileImage(IFormFile file, string userId)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, ImageFolder);
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{userId}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/{ImageFolder}/{uniqueFileName}";
        }
    }
}