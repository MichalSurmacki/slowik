//configuration for infrastructure - called in Startup.cs
using System.Linq;
using Application.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //dependency injection for dbcontextinterface
            services.AddScoped<ISlowikContext>(provider => provider.GetService<SlowikContext>());

            var builder = new SqlConnectionStringBuilder();
            builder.ConnectionString = configuration.GetConnectionString("SlowikConnection");
            builder.UserID = configuration["UserID"];
            builder.Password = configuration["Password"];
            builder.DataSource = configuration["Datasource"] ?? "localhost";
            builder.InitialCatalog = configuration["InitialCatalog"] ?? "slowikDb";

            services.AddDbContext<SlowikContext>(opt => opt.UseSqlServer(builder.ConnectionString));
    
            return services;
        }
    }
}