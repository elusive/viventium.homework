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
            }).Distinct()
            .ToArray();
            for (var i = 0; i < ids.Count(); i++)
                ids[i].EmployeeCount = _context.EmployeeRecords.Count(y => y.CompanyId == ids[i].Id);
            return ids;
        }

        [HttpGet("{id}")]
        public Company GetCompany(int id)
        {
            var firstMatch = _context.EmployeeRecords.First(x => x.CompanyId == id);
            var company = new Company
            {
                Id = firstMatch.CompanyId,
                Code = firstMatch.CompanyCode,
                Description = firstMatch.CompanyDescription,
                Employees = [.. _context.EmployeeRecords
                                .Where(x => x.CompanyId == id)
                                .Select(x => new EmployeeHeader
                                {
                                    EmployeeNumber = x.EmployeeNumber,
                                    FullName = $"{x.EmployeeFirstName} {x.EmployeeLastName}"
                                })]
            };
            company.EmployeeCount = company.Employees.Count();
            return company;
        }

        [HttpGet("{companyId}/Employees/{employeeNumber}")]
        public Employee GetEmployee(int companyId, string employeeNumber)
        {
            var match = _context.EmployeeRecords.First(x => x.EmployeeNumber == employeeNumber && x.CompanyId == companyId); 
            return new Employee
            {
                Email = match.EmployeeEmail,
                Department = match.EmployeeDepartment,
                HireDate = match.HireDate,
                Managers = _context.GetManagers(employeeNumber).ToArray(),
                EmployeeNumber = match.EmployeeNumber,
                FullName = $"{match.EmployeeFirstName} {match.EmployeeLastName}"
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
