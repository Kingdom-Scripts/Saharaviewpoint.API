// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
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
using Newtonsoft.Json.Linq;
using Quartz;
using Saharaviewpoint.Core.BackgroundJobs;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Core.Services;
using Saharaviewpoint.Models.App;
using Saharaviewpoint.Models.Input.Auth;
using Saharaviewpoint.Models.Input.Project;
using Saharaviewpoint.Models.View.Project;
using Saharaviewpoint.Models.View.Task;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using TokenHandler = Saharaviewpoint.Core.Services.TokenHandler;

namespace Saharaviewpoint.Core.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration, bool isProduction)
    {
        // TODO: uncomment the code below to use Azure Key Vault to retrieve secrets
        //// set up database
        //var keyVault = new KeyVaultConfig
        //{
        //    KeyVaultURL = configuration.GetSection("KeyVault:KeyVaultURL").Value,
        //    ClientId = configuration.GetSection("KeyVault:ClientId").Value,
        //    ClientSecret = configuration.GetSection("KeyVault:ClientSecret").Value,
        //    DirectoryID = configuration.GetSection("KeyVault:DirectoryID").Value
        //};

        //var credential = new ClientSecretCredential(keyVault.DirectoryID, keyVault.ClientId, keyVault.ClientSecret);

        //var keyVaultClient = new SecretClient(new Uri(keyVault.KeyVaultURL), credential);

        //string connectionString = keyVaultClient.GetSecret("ConnectionStrings--Saharaviewpoint").Value.Value;

        // TODO: remove the connectionString variable below
        string connectionString = configuration.GetConnectionString("Saharaviewpoint") ?? string.Empty;

        services.AddDbContext<SaharaviewpointContext>((sp, opt) =>
            {
                opt.UseSqlServer(connectionString,
                    b => b.MigrationsAssembly("Saharaviewpoint.API"));
                opt.AddInterceptors(sp.GetRequiredService<SoftDeleteInterceptor>());
                opt.LogTo(Console.WriteLine, LogLevel.Information);
            });

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

            options.AddPolicy("BasicAccess", policy => policy.RequireClaim("SubscriptionPlan", "Basic"));
        });

        // set up Quartz
        services.AddQuartz(q =>
        {
            // Job to notify users of impending task expiration. Runs every day at 12AM
            var reminderJobKey = new JobKey("TaskExpiryReminderJob");
            q.AddJob<TaskExpiryReminderJob>(opts => opts.WithIdentity(reminderJobKey));
            if (isProduction)
            {
                q.AddTrigger(opts => opts
                    .ForJob(reminderJobKey)
                    .WithIdentity("TaskExpiryReminderJob-trigger")
                    .WithCronSchedule("0 0 8 ? * *") // cron job to run every day at 8AM
                    .StartNow()
                );
            }
            else
            {
                q.AddTrigger(opts => opts
                    .ForJob(reminderJobKey)
                    .WithIdentity("TaskExpiryReminderJob-trigger")
                    .WithCronSchedule("* 0/4 * ? * *") // cron job to run every 4 minutes
                    .StartNow()
                );
            }

        });
        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

        // Set up ZeptoMail HttpClient
        string zeptoMailHttpClientName = configuration["ZeptoMail:HttpClientName"]!;
        ArgumentException.ThrowIfNullOrEmpty(zeptoMailHttpClientName);

        string zeptoMailKey = configuration["ZeptoMail:Key"]!;
        ArgumentException.ThrowIfNullOrEmpty(zeptoMailKey);

        // Configure ZeptoMail HttpClient
        services.AddHttpClient(
            zeptoMailHttpClientName,
            client =>
            {
                // Set the base address of the named client.
                client.BaseAddress = new Uri("https://api.zeptomail.com/v1.1/");

                // Add a user-agent default request header.
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Zoho-enczapikey", zeptoMailKey);
            });

        //Mapster global Setting. This can also be overwritten per transform
        TypeAdapterConfig.GlobalSettings.Default
                        .NameMatchingStrategy(NameMatchingStrategy.IgnoreCase)
                        .IgnoreNullValues(true)
                        .AddDestinationTransform((string x) => x.Trim())
                        .AddDestinationTransform((string x) => x ?? "")
                        .AddDestinationTransform(DestinationTransform.EmptyCollectionIfNull);

        // ignore converting Type property in project class
        TypeAdapterConfig<ProjectModel, Project>
            .NewConfig().Ignore(p => p.Type);

        TypeAdapterConfig<TaskComment, TaskCommentView>
            .NewConfig()
            .Map(dest => dest.Children, src => src.Children.Adapt<IEnumerable<TaskCommentView>>());

        TypeAdapterConfig<ProjectTaskApproval, ProjectTaskApprovalView>
            .NewConfig()
            .Map(dest => dest.ProjectTitle, src => src.Project != null ? src.Project.Title : "")
            .Map(dest => dest.RequesterName, src => src.Requester != null ? $"{src.Requester!.FirstName} {src.Requester.LastName}" : "")
            .Map(dest => dest.RequestedOn, src => src.CreatedAt)
            .Map(dest => dest.FulfilledByName, src => src.FulfilledBy != null ? $"{src.FulfilledBy.FirstName} {src.FulfilledBy.LastName}" : null);

        services.TryAddScoped<SoftDeleteInterceptor>();
        services.TryAddScoped<UserSession>();
        services.TryAddScoped<ITokenHandler, TokenHandler>();
        services.TryAddScoped<IFileService, FileService>();
        services.TryAddScoped<IEmailService, EmailService>();

        services.TryAddTransient<IAuthService, AuthService>();
        services.TryAddTransient<IProjectService, ProjectService>();
        services.TryAddTransient<IProjectManagerService, ProjectManagerService>();
        services.TryAddTransient<ITaskService, TaskService>();
        services.TryAddTransient<IClientService, ClientService>();
        services.TryAddTransient<IApprovalService, ApprovalService>();

        return services;
    }
}