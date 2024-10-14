using Microsoft.EntityFrameworkCore;
using SP_000.Models;

namespace SP_000.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        /*** DB Sets ***/
        public DbSet<Employee> Employees { get; set; }
    }
}