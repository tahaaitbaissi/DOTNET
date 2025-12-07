using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CarRental.Desktop.ViewModels.Base
{
    /// <summary>
    /// Classe de base pour tous les ViewModels
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Notifie que la propriété a changé
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Met à jour la valeur et notifie si changement
        /// </summary>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        // ========================================
        // PROPRIÉTÉS UTILES AJOUTÉES
        // ========================================

        private bool _isLoading;
        /// <summary>
        /// Indique si le ViewModel est en train de charger des données
        /// Utilisé pour afficher un spinner
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private string? _errorMessage;
        /// <summary>
        /// Message d'erreur à afficher
        /// </summary>
        public string? ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (SetProperty(ref _errorMessage, value))
                {
                    OnPropertyChanged(nameof(HasError));
                }
            }
        }

        /// <summary>
        /// Indique s'il y a une erreur
        /// </summary>
        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        /// <summary>
        /// Efface le message d'erreur
        /// </summary>
        protected void ClearError()
        {
            ErrorMessage = null;
        }
    }
}