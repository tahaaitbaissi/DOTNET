using System.Threading.Tasks;

namespace CarRental.Desktop.Services
{
    public interface IDialogService
    {
        Task ShowMessageAsync(string title, string message);

        Task<bool> ShowConfirmationAsync(string title, string message);
        Task<string?> ShowInputDialogAsync(string title, string message, string defaultValue = "");
        Task<string?> ShowFilePickerAsync(string filter = "Tous les fichiers (*.*)|*.*");
        Task<string?> ShowFolderPickerAsync();
    }
}