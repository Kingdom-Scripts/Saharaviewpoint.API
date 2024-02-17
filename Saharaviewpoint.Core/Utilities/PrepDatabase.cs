using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Saharaviewpoint.Core.Models.App;
using Saharaviewpoint.Core.Models.App.Constants;
using Serilog;

namespace Saharaviewpoint.Core.Utilities;

public static class PrepDatabase
{
    public static void PrepPopulation(IApplicationBuilder app, bool isProd)
    {
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            SeedData(serviceScope.ServiceProvider.GetService<SaharaviewpointContext>(), isProd);
        }
    }

    private static void SeedData(SaharaviewpointContext context, bool isProd)
    {
        // run migration when in prod
        if (isProd)
        {
            Log.Information("--> Attempting to apply migrations...");
            try
            {
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "--> Could not run migrations.");
            }
        }

        //create default role data
        if (!context.Roles.Any())
        {
            Log.Information("--> Seeding Role Data...");

            // reset the identity count
            context.Database.ExecuteSqlRaw($"DBCC CHECKIDENT ('Roles', RESEED, 0)");

            context.Roles.AddRange(
                new Role { Name = nameof(Roles.SvpAdmin) },
                new Role { Name = nameof(Roles.SvpManager) },
                new Role { Name = nameof(Roles.BusinessAdmin) },
                new Role { Name = nameof(Roles.BusinessManager) },
                new Role { Name = nameof(Roles.BusinessClient) },
                new Role { Name = nameof(Roles.Client) },
                new Role { Name = nameof(Roles.SuperAdmin) }
                );
        }

        context.SaveChanges();
    }
}