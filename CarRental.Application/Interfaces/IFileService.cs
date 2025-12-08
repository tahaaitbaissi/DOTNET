using System.IO;
using System.Threading.Tasks;
using CarRental.Application.Common.Models;

namespace CarRental.Application.Interfaces
{
    public interface IFileService
    {
        Task<Result<string>> UploadFileAsync(Stream fileStream, string fileName, string folderName);
        Task<Result<bool>> DeleteFileAsync(string filePath);
    }
}
