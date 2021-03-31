//configuration for application - called in Startup.cs

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

            return services;
        }
    }
}