using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CarRental.Application.DTOs;
using CarRental.Desktop.Services;

namespace CarRental.Desktop.ViewModels
{
    public class PaymentsViewModel : ViewModelBase
    {
        private readonly IPaymentService _paymentService;
        private ObservableCollection<PaymentDto> _payments;

        public ObservableCollection<PaymentDto> Payments
        {
            get => _payments;
            set => SetProperty(ref _payments, value);
        }

        public PaymentsViewModel(IPaymentService paymentService)
        {
            _paymentService = paymentService;
            Payments = new ObservableCollection<PaymentDto>();
            Title = "Payments";

            _ = LoadPaymentsAsync();
        }

        private async Task LoadPaymentsAsync()
        {
            if (!SessionManager.IsLoggedIn) return;

            IsLoading = true;
            var payments = await _paymentService.GetAllPaymentsAsync();
            Payments = new ObservableCollection<PaymentDto>(payments);
            IsLoading = false;
        }
    }
}
