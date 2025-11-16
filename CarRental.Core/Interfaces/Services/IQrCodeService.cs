using System.Threading.Tasks;

namespace CarRental.Core.Interfaces.Services
{
    public interface IQrCodeService
    {
        Task<byte[]> GenerateQrCodeAsync(string data);
    }
}