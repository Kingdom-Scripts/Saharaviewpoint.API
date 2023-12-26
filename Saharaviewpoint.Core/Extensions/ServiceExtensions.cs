using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Core.Models.App;
using Saharaviewpoint.Core.Models.Input.Auth;
using Saharaviewpoint.Core.Services;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using System.Reflection;
using System.Text;

namespace Saharaviewpoint.Core.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration, bool isProduction)
    {
        // set up database
        //if (isProduction)
        //{
        Console.WriteLine("--> Using SqlServer DB");
        services.AddDbContext<SaharaviewpointContext>(opt =>
        {
            var df = configuration.GetConnectionString("Saharaviewpoint");
            opt.UseSqlServer(configuration.GetConnectionString("Saharaviewpoint"),
                b => b.MigrationsAssembly("Saharaviewpoint.API"));
            opt.LogTo(Console.WriteLine, LogLevel.Information);
        });
        //}
        //else
        //{
        //    Console.WriteLine("--> Using InMemory DB");
        //    services.AddDbContext<SaharaviewpointContext>(opt => opt.UseInMemoryDatabase("InMem"));
        //}

        // Add fluent validation.
        services.AddValidatorsFromAssembly(Assembly.Load("Saharaviewpoint.Core"));
        services.AddFluentValidationAutoValidation(configuration =>
        {
            // Disable the built-in .NET model (data annotations) validation.
            configuration.DisableBuiltInModelValidation = true;

            // Enable validation for parameters bound from `BindingSource.Form` binding sources.
            configuration.EnableFormBindingSourceAutomaticValidation = true;

            // Enable validation for parameters bound from `BindingSource.Path` binding sources.
            configuration.EnablePathBindingSourceAutomaticValidation = true;

            // Enable validation for parameters bound from 'BindingSource.Custom' binding sources.
            configuration.EnableCustomBindingSourceAutomaticValidation = true;

            // Replace the default result factory with a custom implementation.
            configuration.OverrideDefaultResultFactoryWith<CustomResultFactory>();
        });

        services.AddHttpContextAccessor();

        services.AddLazyCache();

        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["JwtConfig:Issuer"],
                ValidAudience = configuration["JwtConfig:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtConfig:Secret"])),
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();
        });

        //Mapster global Setting. This can also be overwritten per transform
        TypeAdapterConfig.GlobalSettings.Default
                        .NameMatchingStrategy(NameMatchingStrategy.IgnoreCase)
                        .IgnoreNullValues(true)
                        .AddDestinationTransform((string x) => x.Trim())
                        .AddDestinationTransform((string x) => x ?? "")
                        .AddDestinationTransform(DestinationTransform.EmptyCollectionIfNull);

        services.AddSingleton<ICacheService, CacheService>();
        services.TryAddScoped<UserSession>();
        services.TryAddScoped<ITokenGenerator, TokenGenerator>();
        services.TryAddTransient<IAuthService, AuthService>();

        return services;
    }
}