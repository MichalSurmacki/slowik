//configuration for application - called in Startup.cs

using System.Reflection;
using Application.Cache;
using Application.Interfaces;
using Application.Repositories;
using Application.Services;
using Application.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICorpusesRepository, CorpusesRepository>();
            services.AddScoped<ICorpusesService, CorpusesService>();
            services.AddScoped<IClarinService, ClarinService>();
            services.AddScoped<ISearchCorpusService, SearchCorpusService>();
            services.AddScoped<IArchiveService, ArchiveService>();
            services.AddSingleton<CorpusesCache>();
            services.AddScoped<ICacheRepository, CacheRepository>();
            services.AddScoped<IEmailService, EmailService>();

            // for emailService - store email and password to smtp in user-secrets or enviroment variables: Email, EmailPassword
            services.Configure<EmailSettings>(optionsSetup =>
            {
                optionsSetup.Email = configuration["Email"];
                optionsSetup.Password = configuration["EmailPassword"];
            });

            // Add caching
            services.AddMemoryCache();

            services.AddHttpClient();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}