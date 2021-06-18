using Microsoft.EntityFrameworkCore;

namespace API_EFCore_AzureFunctions.Models
{
   public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {

        }
        public DbSet<Employee> Employee { get; set; }
    }
}
