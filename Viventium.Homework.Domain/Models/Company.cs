namespace Viventium.Homework.Domain.Models
{
    public class Company: CompanyHeader
    {
        public EmployeeHeader[] Employees { get; set; }
    }
}
