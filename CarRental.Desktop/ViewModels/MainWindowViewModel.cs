using CarRental.Application.DTOs;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CarRental.Desktop.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _message = string.Empty;
        private ObservableCollection<BookingDto> _recentBookings = new(); 

        public event PropertyChangedEventHandler PropertyChanged;

        public string Message
        {
            get => _message;
            set
            {
                if (_message != value)
                {
                    _message = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<BookingDto> RecentBookings 
        {
            get => _recentBookings;
            set
            {
                if (_recentBookings != value)
                {
                    _recentBookings = value;
                    OnPropertyChanged();
                }
            }
        }

        public MainWindowViewModel()
        {
            Message = "Bienvenue dans CarRental Desktop";
            RecentBookings = new ObservableCollection<BookingDto>();

            RecentBookings.Add(new BookingDto
            {
                Id = 1,
                ClientName = "Jean Dupont",
                VehicleName = "Toyota Corolla",
                StartDate = DateTime.Now.AddDays(-2),
                EndDate = DateTime.Now.AddDays(5)
            });
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}