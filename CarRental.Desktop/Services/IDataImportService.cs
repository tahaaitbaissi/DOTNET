using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRental.Desktop.Services
{
    public interface IDataImportService
    {
        // Import simple
        Task<bool> ImportFromCsvAsync(string filePath);
        Task<bool> ImportFromExcelAsync(string filePath);

        // Validation
        Task<List<string>> ValidateImportFileAsync(string filePath);
    }
}