namespace CodeChallenge.Models
{
    public class ReportingStructure
    {
        public Employee Employee { get; set; }

        public int NumberOfReports { get => GetNumberOfReports(Employee); }

        public ReportingStructure(Employee employee)
        {
            this.Employee = employee;
        }

        private int GetNumberOfReports(Employee Employee)
        {
            int counter = 0;
            if (Employee.DirectReports != null)
            {
                foreach (var directReport in Employee.DirectReports)
                {
                    counter++;
                    counter += GetNumberOfReports(directReport);
                }
            }

            return counter;
        }
    }
}
