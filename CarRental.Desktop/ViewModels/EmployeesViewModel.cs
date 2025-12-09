using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CarRental.Application.DTOs;
using CarRental.Desktop.Services;

namespace CarRental.Desktop.ViewModels
{
    public class EmployeesViewModel : ViewModelBase
    {
        private readonly IEmployeeService _employeeService;
        private ObservableCollection<EmployeeDto> _employees;
        private EmployeeDto _selectedEmployee;
        private bool _isEditing;
        private string _editTitle;

        // Form Properties
        private string _fullName;
        private string _position;
        private string _email;

        public ObservableCollection<EmployeeDto> Employees
        {
            get => _employees;
            set => SetProperty(ref _employees, value);
        }

        public EmployeeDto SelectedEmployee
        {
            get => _selectedEmployee;
            set => SetProperty(ref _selectedEmployee, value);
        }

        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }

        public string EditTitle
        {
            get => _editTitle;
            set => SetProperty(ref _editTitle, value);
        }

        public string FullName { get => _fullName; set => SetProperty(ref _fullName, value); }
        public string Position { get => _position; set => SetProperty(ref _position, value); }
        public string Email { get => _email; set => SetProperty(ref _email, value); }

        public ICommand LoadEmployeesCommand { get; }
        public ICommand AddEmployeeCommand { get; }
        public ICommand EditEmployeeCommand { get; }
        public ICommand DeleteEmployeeCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public EmployeesViewModel(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
            _employees = new ObservableCollection<EmployeeDto>();

            LoadEmployeesCommand = new RelayCommand(async _ => await LoadEmployeesAsync());
            AddEmployeeCommand = new RelayCommand(_ => StartEdit(null));
            EditEmployeeCommand = new RelayCommand(_ => StartEdit(SelectedEmployee), _ => SelectedEmployee != null);
            DeleteEmployeeCommand = new RelayCommand(async _ => await DeleteEmployeeAsync(), _ => SelectedEmployee != null);
            SaveCommand = new RelayCommand(async _ => await SaveAsync());
            CancelCommand = new RelayCommand(_ => IsEditing = false);

            _ = LoadEmployeesAsync();
        }

        private async Task LoadEmployeesAsync()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            Employees = new ObservableCollection<EmployeeDto>(employees);
        }

        private void StartEdit(EmployeeDto? employee)
        {
            if (employee == null)
            {
                EditTitle = "Add New Employee";
                FullName = "";
                Position = "Sales";
                Email = "";
                SelectedEmployee = null;
            }
            else
            {
                EditTitle = "Edit Employee";
                FullName = employee.FullName;
                Position = employee.Position;
                Email = employee.Email;
            }
            IsEditing = true;
        }

        private async Task SaveAsync()
        {
            if (SelectedEmployee == null)
            {
                // Create New
                var newEmployee = new CarRental.Application.DTOs.CreateEmployeeDto
                {
                    FullName = FullName,
                    Position = Position,
                    Email = Email, 
                    Username = Email, // Using Email as Username for now
                    Password = "DefaultPassword123!", // Hack for demo
                    // Role = "Employee" // CreateEmployeeDto doesn't have Role, maybe handled by Position or default?
                };
                await _employeeService.AddEmployeeAsync(newEmployee);
                await LoadEmployeesAsync();
            }
            else
            {
                // Update Existing
                SelectedEmployee.FullName = FullName;
                SelectedEmployee.Position = Position;
                SelectedEmployee.Email = Email;

                await _employeeService.UpdateEmployeeAsync(SelectedEmployee.Id, SelectedEmployee);
                
                var index = Employees.IndexOf(SelectedEmployee);
                if (index >= 0) Employees[index] = SelectedEmployee;
            }
            IsEditing = false;
        }

        private async Task DeleteEmployeeAsync()
        {
            if (SelectedEmployee != null)
            {
                await _employeeService.DeleteEmployeeAsync(SelectedEmployee.Id);
                Employees.Remove(SelectedEmployee);
            }
        }
    }
}
