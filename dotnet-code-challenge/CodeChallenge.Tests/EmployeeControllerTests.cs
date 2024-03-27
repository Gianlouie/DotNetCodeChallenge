
using System;
using System.Net;
using System.Net.Http;
using System.Text;

using CodeChallenge.Models;

using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeCodeChallenge.Tests.Integration
{
    [TestClass]
    public class EmployeeControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        // Attribute ClassInitialize requires this signature
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer();
            _httpClient = _testServer.NewClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void CreateEmployee_Returns_Created()
        {
            // Arrange
            var employee = new Employee()
            {
                Department = "Complaints",
                FirstName = "Debbie",
                LastName = "Downer",
                Position = "Receiver",
            };

            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/employee",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newEmployee = response.DeserializeContent<Employee>();
            Assert.IsNotNull(newEmployee.EmployeeId);
            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
            Assert.AreEqual(employee.Department, newEmployee.Department);
            Assert.AreEqual(employee.Position, newEmployee.Position);
        }

        [TestMethod]
        public void GetEmployeeById_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var employee = response.DeserializeContent<Employee>();
            Assert.AreEqual(expectedFirstName, employee.FirstName);
            Assert.AreEqual(expectedLastName, employee.LastName);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_Ok()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f",
                Department = "Engineering",
                FirstName = "Pete",
                LastName = "Best",
                Position = "Developer VI",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var putRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var putResponse = putRequestTask.Result;
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, putResponse.StatusCode);
            var newEmployee = putResponse.DeserializeContent<Employee>();

            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_NotFound()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "Invalid_Id",
                Department = "Music",
                FirstName = "Sunny",
                LastName = "Bono",
                Position = "Singer/Song Writer",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void GetEmployeeReportingStructure_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedNumberOfDirectReports = 4;

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}/reportstructure");
            var response = getRequestTask.Result;
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var reportingStructure = response.DeserializeContent<ReportingStructure>();

            Assert.AreEqual(expectedNumberOfDirectReports, reportingStructure.NumberOfReports);
        }

        [TestMethod]
        public void GetEmployeeReportingStructure_Returns_NotFound()
        {
            // Arrange
            var employeeId = "Bogus id";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}/reportstructure");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void CreateEmployeeCompensation_Returns_Created()
        {

            // Creating employee here as using existing employee from other tests causes issues if tests are run at the same time. 
            var employee = new Employee()
            {
                Department = "department",
                FirstName = "first name",
                LastName = "last name",
                Position = "position",
            };

            var createEmpeRequestContent = new JsonSerialization().ToJson(employee);

            // Execute Create Employee
            var createEmpePostRequestTask = _httpClient.PostAsync("api/employee",
               new StringContent(createEmpeRequestContent, Encoding.UTF8, "application/json"));
            var createEmpeResponse = createEmpePostRequestTask.Result;

            var newEmployee = createEmpeResponse.DeserializeContent<Employee>();

            var compensation = new Compensation()
            {
                EmployeeId = newEmployee.EmployeeId,
                Salary = 1000.00M,
                EffectiveDate = DateTime.Now
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute Create Compensation
            var postRequestTask = _httpClient.PostAsync("api/employee/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
           
            var newCompensation = response.DeserializeContent<Compensation>();
            Assert.AreEqual(compensation.Salary, newCompensation.Salary);
            Assert.AreEqual(compensation.EffectiveDate, newCompensation.EffectiveDate);
        }

        [TestMethod]
        public void GetEmployeeCompensation_Returns_Ok()
        {
            // Arrange
            var employeeId = "62c1084e-6e34-4630-93fd-9153afb65309";

            var expectedCompensation = new Compensation()
            {
                EmployeeId = employeeId,
                Salary = 1000.00M,
                EffectiveDate = DateTime.Now
            };

            var requestContent = new JsonSerialization().ToJson(expectedCompensation);

            // Execute Post to create the compensation record
            var postRequestTask = _httpClient.PostAsync("api/employee/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var postResponse = postRequestTask.Result;

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}/compensation");
            var getResponse = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);

            var compensation = getResponse.DeserializeContent<Compensation>();
            Assert.AreEqual(expectedCompensation.EmployeeId, compensation.EmployeeId);
            Assert.AreEqual(expectedCompensation.Salary, compensation.Salary);
            Assert.AreEqual(expectedCompensation.EffectiveDate, compensation.EffectiveDate);
        }

        [TestMethod]
        public void GetEmployeeCompensation_Returns_NotFound()
        {
            // Arrange
            var employeeId = "62c1084e-6e34-4630-93fd-9153afb65309";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}/compensation");
            var getResponse = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
}
