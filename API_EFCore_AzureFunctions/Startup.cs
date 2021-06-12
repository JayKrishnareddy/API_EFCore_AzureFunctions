using API_EFCore_AzureFunctions.Models;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(API_EFCore_AzureFunctions.Startup))]

namespace API_EFCore_AzureFunctions
{
   public class Startup : FunctionsStartup
   {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string connectionString = "Data Source=DESKTOP-UUBJ14C\\SQLEXPRESS;Integrated Security=true;Database=Sample";
            builder.Services.AddDbContext<AppDbContext>(
                options => SqlServerDbContextOptionsExtensions.UseSqlServer(options, connectionString));
        }
   }
}
