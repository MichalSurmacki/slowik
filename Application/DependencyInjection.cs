//configuration for application - called in Startup.cs

using System.Reflection;
using Application.Interfaces;
using Application.Repositories;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ICorpusesRepository, CorpusesRepository>();
            services.AddScoped<ICorpusesService, CorpusesService>();

            services.AddHttpClient();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}