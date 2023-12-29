using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Saharaviewpoint.Core.Models.App;
using Saharaviewpoint.Core.Models.App.Enums;
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

            context.Roles.AddRange(
                new Role { Name = nameof(Roles.SuperAdmin) },
                new Role { Name = nameof(Roles.Admin) },
                new Role { Name = nameof(Roles.Manager) },
                new Role { Name = nameof(Roles.Business) },
                new Role { Name = nameof(Roles.Client) }
                );
        }

        //// For development mode only
        //if (isProd)
        //{
        //    if (!context.Users.Any())
        //    {
        //        Log.Information("--> Seeding default User data");

        //        var user = new User
        //        {
        //            Uid = new Guid("35ff7ef6-b2a8-4fed-8c2b-fce547207be4"),
        //            Email = "davidire71@gmail.com",
        //            Username = "MO",
        //            Type = UserTypes.Manager,
        //            HashedPassword = "6U2utTVEpDZk56siZhWgWCFPZMwFcsQbaOxDBHiCkNFZgexcm/lflsHPz72SHpe1ZTTeU207jwcEogG7BTQ1RQ==",
        //            IsActive = true
        //        };

        //        context.Add(user);

        //        context.Add(new UserRole
        //        {
        //            User = user,
        //            Role = superAdminRole
        //        });
        //    }
        //}

        context.SaveChanges();
    }
}