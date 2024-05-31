namespace Viventium.Homework.Data.Entities
{
    using CsvHelper.Configuration;
    using CsvHelper.Configuration.Attributes;

    using Microsoft.EntityFrameworkCore;

    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    
    [PrimaryKey(nameof(CompanyId), nameof(EmployeeNumber))]
    public class EmployeeRecord
    {
        public int CompanyId { get; set; }
        public string CompanyCode { get; set; } = "";
        public string CompanyDescription { get; set; } = "";
        public string EmployeeNumber { get; set; } = "";
        public string EmployeeFirstName { get; set; } = "";
        public string EmployeeLastName { get; set; } = "";
        public string EmployeeEmail { get; set; } = "";
        public string EmployeeDepartment { get; set; } = "";
        public DateTime? HireDate { get; set; }
        public string ManagerEmployeeNumber { get; set; } = "";
    }

	public class EmployeeRecordMap : ClassMap<EmployeeRecord>
    {
        public EmployeeRecordMap()
        {
            // Map(m => $"{m.CompanyId}-{m.EmployeeNumber}").Name("Key");
            Map(m => m.CompanyId).Name("CompanyId");
            Map(m => m.CompanyCode).Name("CompanyCode");
            Map(m => m.CompanyDescription).Name("CompanyDescription");
            Map(m => m.EmployeeNumber).Name("EmployeeNumber");
            Map(m => m.EmployeeFirstName).Name("EmployeeFirstName");
            Map(m => m.EmployeeLastName).Name("EmployeeLastName");
            Map(m => m.EmployeeEmail).Name("EmployeeEmail");
            Map(m => m.EmployeeDepartment).Name("EmployeeDepartment");
            Map(m => m.HireDate).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeLocal);
            Map(m => m.ManagerEmployeeNumber).Name("ManagerEmployeeNumber");
        }
    }
}
