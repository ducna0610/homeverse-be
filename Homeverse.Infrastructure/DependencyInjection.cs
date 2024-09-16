using Homeverse.Domain.Interfaces;
using Homeverse.Infrastructure.Data;
using Homeverse.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Homeverse.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<IContactRepository, ContactRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();

        return services;
    }
}
