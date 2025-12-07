using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CarRental.Desktop.ViewModels.Base
{
    /// <summary>
    /// Commande asynchrone pour les opérations qui utilisent async/await
    /// Empêche les double-clics pendant l'exécution
    /// </summary>
    public class AsyncRelayCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool>? _canExecute;
        private bool _isExecuting;

        /// <summary>
        /// Crée une commande asynchrone
        /// </summary>
        /// <param name="execute">Méthode async à exécuter</param>
        /// <param name="canExecute">Condition pour activer la commande (optionnel)</param>
        public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Détermine si la commande peut s'exécuter
        /// Bloque automatiquement pendant l'exécution
        /// </summary>
        public bool CanExecute(object? parameter)
        {
            return !_isExecuting && (_canExecute == null || _canExecute());
        }

        /// <summary>
        /// Exécute la commande de manière asynchrone
        /// </summary>
        public async void Execute(object? parameter)
        {
            if (_isExecuting)
                return;

            _isExecuting = true;
            RaiseCanExecuteChanged();

            try
            {
                await _execute();
            }
            catch (Exception ex)
            {
                // Log l'erreur dans la console de debug
                System.Diagnostics.Debug.WriteLine($"Erreur dans AsyncRelayCommand: {ex.Message}");

                // Vous pouvez aussi afficher un message à l'utilisateur ici
                // ou relancer l'exception pour la gérer dans le ViewModel
                throw;
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Événement déclenché quand CanExecute change
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Force la réévaluation de CanExecute
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}