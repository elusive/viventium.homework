namespace Viventium.Homework.Data
{
    using Microsoft.EntityFrameworkCore;

    using Viventium.Homework.Data.Entities;

    public class CompaniesContext : DbContext
    {
        public CompaniesContext(DbContextOptions<CompaniesContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           // modelBuilder.Entity<EmployeeRecord>().HasPrincipalKey(b => b.Url);
        }

        public DbSet<EmployeeRecord> EmployeeRecords { get; set; }

        public int ResetEmployeeRecords()
        {
            /*
             * NOTE: this deletion method might be preferrable
             * but it is not available in the inmemory database.
            return EmployeeRecords.ExecuteDelete();
            */

            var i = 0;
            var toDelete = EmployeeRecords.ToList();
            for (; i < toDelete.Count(); i++)
            {
                Remove(toDelete[i]);
            }

            SaveChanges();
            return i;
        }

        public bool IsExistingEmployeeId(string id, int companyId)
        {
            var companyEmployees = EmployeeRecords.Where(e => e.CompanyId == companyId);
            return companyEmployees.Any(e => e.EmployeeNumber == id);
        } 
    }
}
