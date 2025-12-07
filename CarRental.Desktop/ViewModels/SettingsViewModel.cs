using CarRental.Desktop.ViewModels.Base;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CarRental.Desktop.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private string _applicationTheme = "Light";
        private bool _autoRefreshEnabled = true;
        private int _refreshInterval = 30;

        public SettingsViewModel()
        {
            // ✅ CORRIGÉ: AsyncRelayCommand pour SaveSettings
            SaveSettingsCommand = new AsyncRelayCommand(SaveSettingsAsync);

            // ✅ CORRIGÉ: RelayCommand synchrone pour ResetSettings
            ResetSettingsCommand = new RelayCommand(_ => ResetSettings());
        }

        public string ApplicationTheme
        {
            get => _applicationTheme;
            set => SetProperty(ref _applicationTheme, value);
        }

        public bool AutoRefreshEnabled
        {
            get => _autoRefreshEnabled;
            set => SetProperty(ref _autoRefreshEnabled, value);
        }

        public int RefreshInterval
        {
            get => _refreshInterval;
            set => SetProperty(ref _refreshInterval, value);
        }

        public ICommand SaveSettingsCommand { get; }
        public ICommand ResetSettingsCommand { get; }

        private async Task SaveSettingsAsync()
        {
            IsLoading = true;
            await Task.Delay(500); 
            IsLoading = false;
        }

        private void ResetSettings()
        {
            ApplicationTheme = "Light";
            AutoRefreshEnabled = true;
            RefreshInterval = 30;
        }
    }
}