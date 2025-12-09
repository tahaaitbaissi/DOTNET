using System.Collections.ObjectModel;
using CarRental.Desktop.Services;

namespace CarRental.Desktop.ViewModels
{
    public class PaymentsViewModel : ViewModelBase
    {
        public ObservableCollection<string> Payments { get; }

        public PaymentsViewModel()
        {
            Payments = new ObservableCollection<string>
            {
                "Payment #1001 - $200.00 - Completed",
                "Payment #1002 - $150.00 - Pending",
                "Payment #1003 - $300.00 - Completed"
            };
        }
    }
}
