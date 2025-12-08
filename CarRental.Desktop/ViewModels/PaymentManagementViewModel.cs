using CarRental.Application.DTOs;
using CarRental.Desktop.Services;
using CarRental.Desktop.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CarRental.Desktop.ViewModels
{
    public class PaymentManagementViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;

        private ObservableCollection<PaymentDto> _payments = new();
        private PaymentDto? _selectedPayment;

        public PaymentManagementViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            // ✅ CORRIGÉ: AsyncRelayCommand
            LoadPaymentsCommand = new AsyncRelayCommand(LoadPaymentsAsync);
            ProcessPaymentCommand = new AsyncRelayCommand(ProcessPaymentAsync);
            RefreshCommand = new AsyncRelayCommand(LoadPaymentsAsync);

            _ = LoadPaymentsAsync();
        }

        public ObservableCollection<PaymentDto> Payments
        {
            get => _payments;
            set => SetProperty(ref _payments, value);
        }

        public PaymentDto? SelectedPayment
        {
            get => _selectedPayment;
            set => SetProperty(ref _selectedPayment, value);
        }

        public ICommand LoadPaymentsCommand { get; }
        public ICommand ProcessPaymentCommand { get; }
        public ICommand RefreshCommand { get; }

        private async Task LoadPaymentsAsync()
        {
            try
            {
                IsLoading = true;
                ClearError();

                await Task.Delay(300);
                // Chargement mock
                Payments = new ObservableCollection<PaymentDto>();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await _dialogService.ShowMessageAsync("Erreur", ErrorMessage);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ProcessPaymentAsync()
        {
            await _dialogService.ShowMessageAsync("Info", "Traitement du paiement");
        }
    }
}