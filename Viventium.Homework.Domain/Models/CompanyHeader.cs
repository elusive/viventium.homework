namespace Viventium.Homework.Domain.Models
{
    public class CompanyHeader
    {
        public int Id { get; set; }             // CompanyId
        public string Code { get; set; }        // CompanyCode
        public string Description { get; set; } // CompanyDescription
        public int EmployeeCount { get; set; }  // # of employees in company
    }
}
