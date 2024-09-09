using AseguradosAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AseguradosAPI.Data
{
    public class AppDbContext :DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Beneficiary> Beneficiaries { get; set; }
        public DbSet<Users> Users { get; set; }
    }
}
