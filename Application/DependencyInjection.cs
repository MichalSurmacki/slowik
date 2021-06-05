//configuration for application - called in Startup.cs

using System.Reflection;
using Application.Cache;
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
            services.AddScoped<IClarinService, ClarinService>();
            services.AddScoped<ISearchCorpusService, SearchCorpusService>();
            services.AddScoped<IArchiveService, ArchiveService>();
            services.AddSingleton<CorpusesCache>();
            services.AddScoped<ICacheRepository, CacheRepository>();

            // Add caching
            services.AddMemoryCache();

            services.AddHttpClient();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}