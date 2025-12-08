using System.Threading.Tasks;

namespace CarRental.Desktop.Services
{
    /// <summary>
    /// Interface complète du service de dialogue
    /// </summary>
    public interface IDialogService
    {
        // Méthodes de base
        Task ShowMessageAsync(string title, string message);
        Task<bool> ShowConfirmationAsync(string title, string message);
        Task<string?> ShowInputDialogAsync(string title, string message, string defaultValue = "");
        Task<string?> ShowFilePickerAsync(string filter = "Tous les fichiers (*.*)|*.*");
        Task<string?> ShowFolderPickerAsync();

        Task ShowInfoAsync(string title, string message);
        Task ShowErrorAsync(string title, string message);
        Task ShowSuccessAsync(string title, string message);
        Task ShowWarningAsync(string title, string message);
    }
}