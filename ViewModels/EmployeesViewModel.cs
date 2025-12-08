using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CarRental.Desktop.Models;
using CarRental.Desktop.Services;

namespace CarRental.Desktop.ViewModels
{
    public class EmployeesViewModel : ViewModelBase
    {
        private readonly IEmployeeService _employeeService;
        private ObservableCollection<Employee> _employees;
        private Employee _selectedEmployee;
        private bool _isEditing;
        private string _editTitle;

        // Form Properties
        private string _firstName;
        private string _lastName;
        private string _role;
        private string _email;
        private string _phone;

        public ObservableCollection<Employee> Employees
        {
            get => _employees;
            set => SetProperty(ref _employees, value);
        }

        public Employee SelectedEmployee
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

        public string FirstName { get => _firstName; set => SetProperty(ref _firstName, value); }
        public string LastName { get => _lastName; set => SetProperty(ref _lastName, value); }
        public string Role { get => _role; set => SetProperty(ref _role, value); }
        public string Email { get => _email; set => SetProperty(ref _email, value); }
        public string Phone { get => _phone; set => SetProperty(ref _phone, value); }

        public ICommand LoadEmployeesCommand { get; }
        public ICommand AddEmployeeCommand { get; }
        public ICommand EditEmployeeCommand { get; }
        public ICommand DeleteEmployeeCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public EmployeesViewModel(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
            _employees = new ObservableCollection<Employee>();

            LoadEmployeesCommand = new RelayCommand(async _ => await LoadEmployeesAsync());
            AddEmployeeCommand = new RelayCommand(_ => StartEdit(null));
            EditEmployeeCommand = new RelayCommand(_ => StartEdit(SelectedEmployee), _ => SelectedEmployee != null);
            DeleteEmployeeCommand = new RelayCommand(async _ => await DeleteEmployeeAsync(), _ => SelectedEmployee != null);
            SaveCommand = new RelayCommand(async _ => await SaveAsync());
            CancelCommand = new RelayCommand(_ => IsEditing = false);

            LoadEmployeesAsync();
        }

        private async Task LoadEmployeesAsync()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            Employees = new ObservableCollection<Employee>(employees);
        }

        private void StartEdit(Employee? employee)
        {
            if (employee == null)
            {
                EditTitle = "Add New Employee";
                FirstName = "";
                LastName = "";
                Role = "Sales";
                Email = "";
                Phone = "";
                SelectedEmployee = null;
            }
            else
            {
                EditTitle = "Edit Employee";
                FirstName = employee.FirstName;
                LastName = employee.LastName;
                Role = employee.Role;
                Email = employee.Email;
                Phone = employee.Phone;
            }
            IsEditing = true;
        }

        private async Task SaveAsync()
        {
            if (SelectedEmployee == null)
            {
                var newEmployee = new Employee
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Role = Role,
                    Email = Email,
                    Phone = Phone
                };
                await _employeeService.AddEmployeeAsync(newEmployee);
                Employees.Add(newEmployee);
            }
            else
            {
                SelectedEmployee.FirstName = FirstName;
                SelectedEmployee.LastName = LastName;
                SelectedEmployee.Role = Role;
                SelectedEmployee.Email = Email;
                SelectedEmployee.Phone = Phone;

                var index = Employees.IndexOf(SelectedEmployee);
                Employees[index] = SelectedEmployee;
            }
            IsEditing = false;
        }

        private async Task DeleteEmployeeAsync()
        {
            if (SelectedEmployee != null)
            {
                Employees.Remove(SelectedEmployee);
                await _employeeService.DeleteEmployeeAsync(SelectedEmployee.Id);
            }
        }
    }
}
