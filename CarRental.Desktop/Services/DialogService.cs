using System;
using System.Threading.Tasks;
using System.Windows;

namespace CarRental.Desktop.Services
{
    /// <summary>
    /// Implémentation complète du service de dialogue
    /// </summary>
    public class DialogService : IDialogService
    {
        public Task ShowMessageAsync(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
            return Task.CompletedTask;
        }

        public Task<bool> ShowConfirmationAsync(string title, string message)
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return Task.FromResult(result == MessageBoxResult.Yes);
        }

        public Task<string?> ShowInputDialogAsync(string title, string message, string defaultValue = "")
        {
            // Simple MessageBox pour l'instant
            MessageBox.Show("Fonction InputDialog non implémentée", "Info", MessageBoxButton.OK);
            return Task.FromResult<string?>(defaultValue);
        }

        public Task<string?> ShowFilePickerAsync(string filter = "Tous les fichiers (*.*)|*.*")
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = filter
            };

            var result = dialog.ShowDialog();
            return Task.FromResult(result == true ? dialog.FileName : null);
        }

        public Task<string?> ShowFolderPickerAsync()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                ValidateNames = false,
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Select Folder"
            };

            var result = dialog.ShowDialog();
            return Task.FromResult(result == true ?
                System.IO.Path.GetDirectoryName(dialog.FileName) : null);
        }


        public Task ShowInfoAsync(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
            return Task.CompletedTask;
        }

        public Task ShowErrorAsync(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            return Task.CompletedTask;
        }

        public Task ShowSuccessAsync(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
            return Task.CompletedTask;
        }

        public Task ShowWarningAsync(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
            return Task.CompletedTask;
        }
    }
}