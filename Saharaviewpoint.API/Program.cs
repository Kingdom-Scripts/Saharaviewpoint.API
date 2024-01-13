using Microsoft.OpenApi.Models;
using Saharaviewpoint.Core.Extensions;
using Saharaviewpoint.Core.Middlewares;
using Saharaviewpoint.Core.Models.Configurations;
using Saharaviewpoint.Core.Utilities;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Application Starting up...");

var builder = WebApplication.CreateBuilder(args);

// set up serilog.
builder.Host.UseSerilog((hostContext, services, config) =>
{
    config.ReadFrom.Configuration(hostContext.Configuration);
    config.ReadFrom.Services(services);
    config.Enrich.FromLogContext();
    config.WriteTo.Console();
});

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
{
    //This is to generate the Default UI of Swagger Documentation
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Saharaviewpoint API",
        Description = "Saharaviewpoint Web API. A project for managing and tracking construction development.",
        Contact = new OpenApiContact
        {
            Name = "Saharaviewpoint",
            Email = "contact@saharaviewpoint.com"
        }
    });

    string xmlFilePath = Path.Combine(AppContext.BaseDirectory, "Saharaviewpoint.xml");
    swagger.IncludeXmlComments(xmlFilePath, true);

    // include the XML of Saharaviewpoint.Core
    string coreXmlFilePath = Path.Combine(AppContext.BaseDirectory, "SaharaviewpointCore.xml");
    swagger.IncludeXmlComments(coreXmlFilePath, true);

    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\""
    });

    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
        }
    });

    swagger.IgnoreObsoleteActions();
    swagger.IgnoreObsoleteProperties();
});

builder.Services.Configure<AppConfig>(builder.Configuration.GetSection("AppConfig"));
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));
builder.Services.Configure<KeyVaultConfig>(builder.Configuration.GetSection("KeyVault"));

builder.Services.ConfigureServices(builder.Configuration, builder.Environment.IsProduction());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<JWTMiddleware>();

app.UseMiddleware<UserSessionMiddleware>();

app.MapControllers();

PrepDatabase.PrepPopulation(app, app.Environment.IsProduction());

app.Run();

Log.Information("Application Stopped cleanly");

Log.CloseAndFlush();