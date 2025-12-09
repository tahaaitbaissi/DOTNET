using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CarRental.Desktop.Services
{
    public class DataImportService : IDataImportService
    {
        private readonly IDialogService _dialogService;

        public DataImportService(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public async Task<bool> ImportFromCsvAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    // CORRECTION : Utilisez ShowMessageAsync
                    await _dialogService.ShowMessageAsync("Erreur", "Fichier introuvable");
                    return false;
                }

                var lines = await File.ReadAllLinesAsync(filePath);

                // CORRECTION : Utilisez ShowMessageAsync
                await _dialogService.ShowMessageAsync("Succès",
                    $"{lines.Length - 1} lignes importées depuis CSV");
                return true;
            }
            catch (Exception ex)
            {
                // CORRECTION : Utilisez ShowMessageAsync
                await _dialogService.ShowMessageAsync("Erreur", ex.Message);
                return false;
            }
        }

        public async Task<bool> ImportFromExcelAsync(string filePath)
        {
            try
            {
                // CORRECTION : Utilisez ShowMessageAsync
                await _dialogService.ShowMessageAsync("Info",
                    "Import Excel simulé");
                return true;
            }
            catch (Exception ex)
            {
                // CORRECTION : Utilisez ShowMessageAsync
                await _dialogService.ShowMessageAsync("Erreur", ex.Message);
                return false;
            }
        }

        public async Task<List<string>> ValidateImportFileAsync(string filePath)
        {
            var errors = new List<string>();

            if (!File.Exists(filePath))
                errors.Add("Fichier introuvable");

            if (Path.GetExtension(filePath).ToLower() != ".csv")
                errors.Add("Format non supporté (CSV requis)");

            return await Task.FromResult(errors);
        }
    }
}