namespace Viventium.Homework.Api.Controllers
{
    using CsvHelper;
    using CsvHelper.Configuration;

    using Microsoft.AspNetCore.Mvc;

    using System.Globalization;
    using System.Text;

    using Viventium.Homework.Data;
    using Viventium.Homework.Data.Entities;
    using Viventium.Homework.Domain.Models;

    [ApiController]
    [Route("[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly CompaniesContext _context;

        private readonly ILogger<CompaniesController> _logger;

        public CompaniesController(ILogger<CompaniesController> logger, CompaniesContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IEnumerable<CompanyHeader> GetCompanies()
        {
            var ids = _context.EmployeeRecords.Select(x => new CompanyHeader
            {
                Id = x.CompanyId,
                Code = x.CompanyCode,
                Description = x.CompanyDescription,
            }).Distinct();
            foreach (var c in ids)
                c.EmployeeCount = _context.EmployeeRecords.Count(y => y.CompanyId == c.Id);
            return ids;
        }

        [HttpGet("{companyId}")]
        public Company GetCompany(int id)
        {
            return new Company
            {
                Employees = new EmployeeHeader[]
                {
                    new EmployeeHeader { EmployeeNumber = "E2345", FullName = "John Johnson" }
                }
            };
        }

        [HttpGet("{companyId}/Employees/{employeeNumber}")]
        public Employee GetEmployee(int companyId, string employeeNumber)
        {
            return new Employee
            {
                Email = "me@johng.info",
                Department = "Engineering",
                HireDate = new DateTime(1974, 7, 2),
                Managers = new EmployeeHeader[]
                {
                    new EmployeeHeader { EmployeeNumber = "E34567", FullName = "Joe Manager" }
                }
            };
        }

        [HttpPost()]
        [Route("/DataStore")]
        public async Task<IActionResult> PostData()
        {
            var cnt = 0;
            using (var tr = new StreamReader(Request.Body, encoding: Encoding.UTF8))
            using (var csv = new CsvReader(tr, CultureInfo.InvariantCulture))
            {

                // empty current records
                var deletedCount = _context.ResetEmployeeRecords();
                _logger.LogInformation($"Deleted {deletedCount} records.");

                // add new records
                EmployeeRecord holder = new EmployeeRecord();
                await foreach (var record in csv.EnumerateRecordsAsync<EmployeeRecord>(holder))
                {
                    _context.EmployeeRecords.Add(record);
                    _context.SaveChanges();
                    _logger.LogInformation($"Added Record {cnt++}: {record?.EmployeeFirstName} {record?.EmployeeLastName}");
                }
            }

            return Ok($"{cnt} records saved");
        }
    }
}
