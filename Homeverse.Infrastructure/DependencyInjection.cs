﻿using Hangfire;
using Homeverse.Application.Interfaces;
using Homeverse.Domain.Interfaces;
using Homeverse.Infrastructure.Data;
using Homeverse.Infrastructure.Repositories;
using Homeverse.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

        services.AddDbContext<HomeverseDbContext>(options =>
        {
            options.UseSqlServer(config.GetConnectionString("Database"));
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        services.AddHangfire(configuration =>
        {
            configuration.UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(config.GetConnectionString("Database"));
        });
        services.AddHangfireServer();

        services.AddStackExchangeRedisCache(option =>
        {
            option.Configuration = config.GetConnectionString("Redis");
        });
        services.AddSingleton<ICacheService, CacheService>();
        services.Configure<MailSettings>(config.GetSection("MailSettings"));
        services.AddSingleton<IMailService, MailService>();
        services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
        services.AddSingleton<IFileStorageService, FileStorageService>();

        var secretKey = config.GetSection("JwtSettings:SecretKey").Value;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidIssuer = config.GetSection("JwtSettings:Issuer").Value,
                ValidateAudience = true,
                ValidAudience = config.GetSection("JwtSettings:Audience").Value,
                IssuerSigningKey = key
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];

                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }
}
