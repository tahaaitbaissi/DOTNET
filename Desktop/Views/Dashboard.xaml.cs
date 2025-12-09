using System.Windows;
using System.Windows.Controls;

namespace YourNamespace
{
    public partial class Dashboard : Page
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new DashboardView();
        }

        private void BtnVehicles_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new VehiclesView();
        }

        private void BtnClients_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ClientsView();
        }

        private void BtnPayments_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new PaymentsView();
        }

        private void BtnReservations_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ReservationsView();
        }

        private void BtnReturns_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ReturnsView();
        }

        private void BtnContracts_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ContractsView();
        }

        private void BtnReports_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ReportsView();
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new SettingsView();
        }
    }
}