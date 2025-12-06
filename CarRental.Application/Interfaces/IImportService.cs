using CarRental.Application.Common.Models;
using System.IO;
using System.Threading.Tasks;

namespace CarRental.Application.Interfaces
{
    public interface IImportService
    {
        /// <summary>
        /// Imports vehicles from a CSV file stream.
        /// </summary>
        /// <param name="fileStream">The CSV file content</param>
        /// <returns>Result containing success count and messages</returns>
        Task<Result<ImportResultDto>> ImportVehiclesAsync(Stream fileStream);
    }

    public class ImportResultDto
    {
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public string Message { get; set; }
    }
}
