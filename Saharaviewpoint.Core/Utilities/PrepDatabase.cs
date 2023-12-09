using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Saharaviewpoint.Core.Models.App;
using Serilog;

namespace Saharaviewpoint.Core.Utilities;

public static class PrepDatabase
{
    public static void PrepPopulation(IApplicationBuilder app, bool isProd)
    {
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            SeedData(serviceScope.ServiceProvider.GetService<ShareviewpointContext>(), isProd);
        }
    }

    private static void SeedData(ShareviewpointContext context, bool isProd)
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

        //create default data
        if (!context.Roles.Any())
        {
            Log.Information("--> Seeding Role Data...");

            context.Roles.AddRange(
                new Role { Name = "Admin" },
                new Role { Name = "Manager" },
                new Role { Name = "Business" },
                new Role { Name = "Client" }
                );

            context.SaveChanges();
        }
    }
}