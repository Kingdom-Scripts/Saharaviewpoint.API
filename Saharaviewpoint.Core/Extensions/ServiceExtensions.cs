// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

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
using Quartz;
using Saharaviewpoint.Core.BackgroundJobs;
using Saharaviewpoint.Core.Contants;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Core.Middlewares;
using Saharaviewpoint.Core.Services;
using Saharaviewpoint.Core.Utilities;
using Saharaviewpoint.Models.App;
using Saharaviewpoint.Models.Input.Auth;
using Saharaviewpoint.Models.Input.Project;
using Saharaviewpoint.Models.Utilities;
using Saharaviewpoint.Models.View.Project;
using Saharaviewpoint.Models.View.Task;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using TokenHandler = Saharaviewpoint.Core.Services.TokenHandler;

namespace Saharaviewpoint.Core.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration, bool isProduction)
    {
        var keyVault = new KeyVaultUtil(configuration.GetSection("AppConfig:KeyVaultUrl").Value ?? string.Empty);
        string connectionString = keyVault.GetSecret(KeyVaultKeys.DefaultConnectionString);
        services.AddDbContext<SaharaviewpointContext>((sp, opt) =>
            {
                opt.UseSqlServer(connectionString,
                    b => b.MigrationsAssembly("Saharaviewpoint.API"));
                opt.AddInterceptors(sp.GetRequiredService<SoftDeleteInterceptor>());
                opt.LogTo(Console.WriteLine, LogLevel.Information);
            });

        // Add fluent validation.
        services.AddValidatorsFromAssembly(Assembly.Load("Saharaviewpoint.Core"));
        services.AddFluentValidationAutoValidation(config =>
        {
            // Disable the built-in .NET model (data annotations) validation.
            config.DisableBuiltInModelValidation = true;

            // Enable validation for parameters bound from `BindingSource.Form` binding sources.
            config.EnableFormBindingSourceAutomaticValidation = true;

            // Enable validation for parameters bound from `BindingSource.Path` binding sources.
            config.EnablePathBindingSourceAutomaticValidation = true;

            // Enable validation for parameters bound from 'BindingSource.Custom' binding sources.
            config.EnableCustomBindingSourceAutomaticValidation = true;

            // Replace the default result factory with a custom implementation.
            config.OverrideDefaultResultFactoryWith<CustomResultFactory>();
        });

        services.AddHttpContextAccessor();

        services.AddLazyCache();

        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(bearerOptions =>
        {
            bearerOptions.SaveToken = true;
            bearerOptions.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["JwtConfig:Issuer"],
                ValidAudience = configuration["JwtConfig:Audience"],
                IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyVault.GetSecret(KeyVaultKeys.JwtSecert))),
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization(authorizationOptions =>
        {
            authorizationOptions.FallbackPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();
        });

        // Add HTTP client for Api Video
        services.AddHttpClient(HttpClientKeys.ApiVideo, client =>
        {
            string baseAddress = configuration["AppConfig:ApiVideoUrl"]!;

            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        })
        .AddHttpMessageHandler<ApiVideoHttpHandler>()
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                UseDefaultCredentials = true
            });

        // Set up ZeptoMail HttpClient
        string zeptoMailKey = keyVault.GetSecret(KeyVaultKeys.ZeptoMailKey);
        ArgumentException.ThrowIfNullOrEmpty(zeptoMailKey);

        // Configure ZeptoMail HttpClient
        services.AddHttpClient(
            HttpClientKeys.ZeptoMail,
            client =>
            {
                // Set the base address of the named client.
                client.BaseAddress = new Uri("https://api.zeptomail.com/v1.1/");

                // Add a user-agent default request header.
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Zoho-enczapikey", zeptoMailKey);
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
                    //.WithCronSchedule("* 0/4 * ? * *") // cron job to run every 4 minutes
                    .WithCronSchedule("0 0 8 ? * *") // cron job to run every day at 8AM

                    .StartNow()
                );
            }

        });
        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

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

        services.TryAddSingleton<ScopedSecrets>();

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
        services.TryAddTransient<ApiVideoHttpHandler>();

        return services;
    }
}