using System.ComponentModel;

namespace CarRental.Desktop.Services;

public class AppState : INotifyPropertyChanged
{
    private static AppState? _instance;
    public static AppState Instance => _instance ??= new AppState();

    private bool _isLoading;
    private string _currentPage = "Dashboard";

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            if (_isLoading != value)
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }
    }

    public string CurrentPage
    {
        get => _currentPage;
        set
        {
            if (_currentPage != value)
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private AppState() { }
}