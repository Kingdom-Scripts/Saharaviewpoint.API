using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Core.Models.App;
using Saharaviewpoint.Core.Services;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Enums;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using System.Reflection;

namespace Saharaviewpoint.Core.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration, bool isProduction)
    {
        // set up database 
        if (isProduction)
        {
            Console.WriteLine("--> Using SqlServer DB");
            services.AddDbContext<SaharaviewpointContext>(opt =>
            {
                var df = configuration.GetConnectionString("Saharaviewpoint");
                opt.UseSqlServer(configuration.GetConnectionString("Saharaviewpoint"));
                opt.LogTo(Console.WriteLine, LogLevel.Information);
            });
        }
        else
        {
            Console.WriteLine("--> Using InMemory DB");
            services.AddDbContext<SaharaviewpointContext>(opt => opt.UseInMemoryDatabase("InMem"));
        }

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

        //Mapster global Setting. This can also be overwritten per transform
        TypeAdapterConfig.GlobalSettings.Default
                        .NameMatchingStrategy(NameMatchingStrategy.IgnoreCase)
                        .IgnoreNullValues(true)
                        .AddDestinationTransform((string x) => x.Trim())
                        .AddDestinationTransform((string x) => x ?? "")
                        .AddDestinationTransform(DestinationTransform.EmptyCollectionIfNull);

        services.AddSingleton<ICacheService, CacheService>();
        services.TryAddScoped<ITokenGenerator, TokenGenerator>();
        services.TryAddTransient<IAuthService, AuthService>();

        return services;
    }
}