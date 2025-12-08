using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CarRental.Desktop.Models;
using CarRental.Desktop.Services;

namespace CarRental.Desktop.ViewModels
{
    public class RentalsViewModel : ViewModelBase
    {
        private readonly IRentalService _rentalService;
        private ObservableCollection<Rental> _rentals;
        private Rental _selectedRental;

        public ObservableCollection<Rental> Rentals
        {
            get => _rentals;
            set => SetProperty(ref _rentals, value);
        }

        public Rental SelectedRental
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
            _rentals = new ObservableCollection<Rental>();

            LoadRentalsCommand = new RelayCommand(async _ => await LoadRentalsAsync());
            CompleteRentalCommand = new RelayCommand(async _ => await UpdateStatusAsync("Completed"), _ => SelectedRental != null && SelectedRental.Status == "Active");
            CancelRentalCommand = new RelayCommand(async _ => await UpdateStatusAsync("Cancelled"), _ => SelectedRental != null && SelectedRental.Status == "Active");

            LoadRentalsAsync();
        }

        private async Task LoadRentalsAsync()
        {
            var rentals = await _rentalService.GetAllRentalsAsync();
            Rentals = new ObservableCollection<Rental>(rentals);
        }

        private async Task UpdateStatusAsync(string newStatus)
        {
            if (SelectedRental != null)
            {
                SelectedRental.Status = newStatus;
                // Trigger UI update
                var index = Rentals.IndexOf(SelectedRental);
                Rentals[index] = SelectedRental;
                
                // await _rentalService.UpdateRentalStatusAsync(SelectedRental.Id, newStatus);
            }
        }
    }
}
