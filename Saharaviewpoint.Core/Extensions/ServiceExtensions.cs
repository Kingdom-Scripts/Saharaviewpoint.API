using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Core.Models.App;
using Saharaviewpoint.Core.Services;

namespace Saharaviewpoint.Core.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration, bool isProduction)
    {
        // set up database 
        if (isProduction)
        {
            Console.WriteLine("--> Using SqlServer DB");
            services.AddDbContext<ShareviewpointContext>(opt =>
            {
                var df = configuration.GetConnectionString("Shareviewpoint");
                opt.UseSqlServer(configuration.GetConnectionString("Shareviewpoint"));
                opt.LogTo(Console.WriteLine, LogLevel.Information);
            });
        }
        else
        {
            Console.WriteLine("--> Using InMemory DB");
            services.AddDbContext<ShareviewpointContext>(opt => opt.UseInMemoryDatabase("InMem"));
        }


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

        return services;
    }
}