namespace Viventium.Homework.Domain.Models
{
    public class Employee
    {
        public string Email { get; set; }
        public string Department { get; set; }
        public DateTime? HireDate { get; set; }
        public EmployeeHeader[] Managers { get; set; }   // list of employee manager headers by seniority desc.
    }
}
