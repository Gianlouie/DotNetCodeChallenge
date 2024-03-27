using System;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Data;

namespace CodeChallenge.Repositories
{
    public class EmployeeRespository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRespository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Employee Add(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid().ToString();
            _employeeContext.Employees.Add(employee);
            return employee;
        }

        public Employee GetById(string id)
        {
            // Adding a ToList() here fixes a weird bug where the employeeContext will actually return an Employee object with a null Direct Reports list for some reason
            // My guess is a bug with in memory databases interacting with EF
            return _employeeContext.Employees.ToList().SingleOrDefault(e => e.EmployeeId == id);
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }

        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }

        public Compensation AddCompensation(Compensation compensation)
        {
            compensation.Id = Guid.NewGuid().ToString();

            _employeeContext.Compensations.Add(compensation);

            return compensation;
        }

        public Compensation GetCompensationByEmployeeId(string id)
        {
            return _employeeContext.Compensations.ToList().SingleOrDefault(e => e.EmployeeId == id);
        }
    }
}
