using Homeverse.Application.Interfaces;
using Homeverse.Domain.Interfaces;
using Homeverse.Infrastructure.Data;
using Homeverse.Infrastructure.Repositories;
using Homeverse.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Homeverse.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<IContactRepository, ContactRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();

        services.AddStackExchangeRedisCache(option =>
        {
            option.Configuration = config.GetConnectionString("Redis");
        });
        services.AddSingleton<ICacheService, CacheService>();
        services.Configure<MailSettings>(config.GetSection("MailSettings"));
        services.AddSingleton<IMailService, MailService>();

        return services;
    }
}
