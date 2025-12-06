using CarRental.Desktop.ViewModels.Base;
using System.Windows.Input;

namespace CarRental.Desktop.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private string _applicationTheme = "Light";
    private bool _autoRefreshEnabled = true;
    private int _refreshInterval = 30;

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

    public SettingsViewModel()
    {
        SaveSettingsCommand = new RelayCommand(async (param) => await SaveSettingsAsync());
        ResetSettingsCommand = new RelayCommand((param) => ResetSettings());

        LoadSettings();
    }

    private void LoadSettings()
    {
        // TODO: Charger depuis un fichier de configuration
        // ApplicationTheme = Properties.Settings.Default.Theme;
        // AutoRefreshEnabled = Properties.Settings.Default.AutoRefresh;
        // RefreshInterval = Properties.Settings.Default.RefreshInterval;
    }

    private async Task SaveSettingsAsync()
    {
        // TODO: Sauvegarder les paramètres
        // Properties.Settings.Default.Theme = ApplicationTheme;
        // Properties.Settings.Default.AutoRefresh = AutoRefreshEnabled;
        // Properties.Settings.Default.RefreshInterval = RefreshInterval;
        // Properties.Settings.Default.Save();

        await Task.CompletedTask;
    }

    private void ResetSettings()
    {
        ApplicationTheme = "Light";
        AutoRefreshEnabled = true;
        RefreshInterval = 30;
    }
}