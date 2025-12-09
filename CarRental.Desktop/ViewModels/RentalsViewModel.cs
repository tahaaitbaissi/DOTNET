using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CarRental.Application.DTOs;
using CarRental.Desktop.Services;

namespace CarRental.Desktop.ViewModels
{
    public class RentalsViewModel : ViewModelBase
    {
        private readonly IRentalService _rentalService;
        private ObservableCollection<BookingDto> _rentals;
        private BookingDto _selectedRental;

        public ObservableCollection<BookingDto> Rentals
        {
            get => _rentals;
            set => SetProperty(ref _rentals, value);
        }

        public BookingDto SelectedRental
        {
            get => _selectedRental;
            set => SetProperty(ref _selectedRental, value);
        }

        public ICommand LoadRentalsCommand { get; }
        public ICommand CompleteRentalCommand { get; }
        public ICommand CancelRentalCommand { get; }

        public RentalsViewModel(IRentalService rentalService)
        {
            _rentalService = rentalService;
            _rentals = new ObservableCollection<BookingDto>();

            LoadRentalsCommand = new RelayCommand(async _ => await LoadRentalsAsync());
            CompleteRentalCommand = new RelayCommand(async _ => await UpdateStatusAsync("Completed"), _ => SelectedRental != null && SelectedRental.Status == "Active");
            CancelRentalCommand = new RelayCommand(async _ => await UpdateStatusAsync("Cancelled"), _ => SelectedRental != null && SelectedRental.Status == "Active");

            _ = LoadRentalsAsync();
        }

        private async Task LoadRentalsAsync()
        {
            var rentals = await _rentalService.GetAllRentalsAsync();
            Rentals = new ObservableCollection<BookingDto>(rentals);
        }

        private async Task UpdateStatusAsync(string newStatus)
        {
            if (SelectedRental != null)
            {
                // In DTO, changing property locally, then call service
                SelectedRental.Status = newStatus;
                
                await _rentalService.UpdateRentalStatusAsync(SelectedRental.Id, newStatus);
                
                // Trigger UI update (refresh list or create new collection to force notify)
                // Simply notifying isn't enough in Collection unless item implements INotifyPropertyChanged.
                // DTO probably doesn't.
                var index = Rentals.IndexOf(SelectedRental);
                if (index >= 0)
                {
                    Rentals[index] = SelectedRental; // Force refresh
                }
            }
        }
    }
}
