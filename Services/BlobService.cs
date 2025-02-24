using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;

namespace habyx.Services
{
    public class BlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public BlobService(IConfiguration configuration)
        {
            _blobServiceClient = new BlobServiceClient(configuration.GetConnectionString("AzureStorage"));
            _containerName = "profileimages";
        }

        public async Task<string> UploadImageAsync(IFormFile file, string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blobClient = containerClient.GetBlobClient(fileName);
            var blobHttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType };
            
            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = blobHttpHeaders });

            return blobClient.Uri.ToString();
        }

        public async Task DeleteImageAsync(string imageUrl)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var fileName = Path.GetFileName(new Uri(imageUrl).LocalPath);
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.DeleteIfExistsAsync();
        }
    }
}