using CarRental.Application.Common.Models;
using CarRental.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CarRental.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<Result<string>> UploadFileAsync(Stream fileStream, string fileName, string folderName)
        {
            if (fileStream == null || fileStream.Length == 0)
            {
                return Result<string>.Failure("No file uploaded.");
            }

            // Validate extension
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".webp"))
            {
                return Result<string>.Failure("Invalid file type. Only JPG, PNG, and WebP are allowed.");
            }

            // Ensure directory exists
            var uploadsRoot = Path.Combine(_environment.WebRootPath, "images", folderName);
            if (!Directory.Exists(uploadsRoot))
            {
                Directory.CreateDirectory(uploadsRoot);
            }

            // Generate unique filename
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsRoot, uniqueFileName);

            // Save file
            try
            {
                using (var fileStreamOutput = new FileStream(filePath, FileMode.Create))
                {
                    await fileStream.CopyToAsync(fileStreamOutput);
                }
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"File upload failed: {ex.Message}");
            }

            // Return relative path for DB
            var relativePath = $"/images/{folderName}/{uniqueFileName}";
            return Result<string>.Success(relativePath);
        }

        public Task<Result<bool>> DeleteFileAsync(string relativePath)
        {
            try
            {
                // Remove leading slash if present
                if (relativePath.StartsWith("/")) relativePath = relativePath.Substring(1);

                var fullPath = Path.Combine(_environment.WebRootPath, relativePath);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return Task.FromResult(Result<bool>.Success(true));
                }

                return Task.FromResult(Result<bool>.Failure("File not found."));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result<bool>.Failure($"File deletion failed: {ex.Message}"));
            }
        }
    }
}
