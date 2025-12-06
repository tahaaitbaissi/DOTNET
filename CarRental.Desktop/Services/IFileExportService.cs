using CarRental.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRental.Desktop.Services;

public interface IFileExportService
{
    Task<bool> ExportToExcelAsync<T>(IEnumerable<T> data, string filePath, string sheetName = "Data");
    Task<bool> ExportClientsAsync(IEnumerable<ClientDto> clients, string filePath);
    Task<bool> ExportVehiclesAsync(IEnumerable<VehicleDto> vehicles, string filePath);
    Task<bool> ExportBookingsAsync(IEnumerable<BookingDto> bookings, string filePath);
}