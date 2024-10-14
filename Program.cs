using System.Net;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SP_000;
using SP_000.Data;
using SP_000.Repositories;
using SP_000.Repositories.Interfaces;
using SP_000.Services.HealthChecks;


/* .ENV Loading */
var root = Directory.GetCurrentDirectory();
var dotenvFile = Path.Combine(root, ".env");
DotEnv.Load(dotenvFile);

var builder = WebApplication.CreateBuilder(args);

/*** Add Configurations to the Container ***/
builder.Configuration.AddEnvironmentVariables();

/*** Configure Kestrel Endpoints ***/
builder.WebHost.UseKestrel(options =>
    {
        int httpPort;
        if (builder.Environment.IsDevelopment())
        {
            httpPort = 5012;
        }
        else
        {
            httpPort = 8080;
        }

        options.Listen(IPAddress.Any, httpPort);
    }
);

/*** Add Services to the Container ***/
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("MSSQL Health Check", HealthStatus.Unhealthy)
    .AddCheck<DbHealthCheck>("MSSQL Custom Health Check", HealthStatus.Unhealthy);
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContext<AppDbContext>(options =>
    {
        if (builder.Environment.IsProduction())
        {
            var server = Environment.GetEnvironmentVariable("MSSQL_DATABASE_SERVER");
            var port = Environment.GetEnvironmentVariable("MSSQL_DATABASE_PORT");
            var database = Environment.GetEnvironmentVariable("MSSQL_DATABASE_DATABASE");
            var username = Environment.GetEnvironmentVariable("MSSQL_DATABASE_USERNAME");
            var password = Environment.GetEnvironmentVariable("MSSQL_DATABASE_PASSWORD");

            options.UseLazyLoadingProxies().UseSqlServer(
                $"Server={server},{port};Database={database};User ID={username};Password={password};Trusted_Connection=False;TrustServerCertificate=True"
            );
        }
        if (builder.Environment.IsDevelopment())
        {
            options.UseLazyLoadingProxies().UseInMemoryDatabase("InMem");
        }
    }
);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
            builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

/*** Configure the HTTP request pipeline ***/
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.UseRouting();
app.UseCors("AllowAll");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

/*** Initial Data Seeding ***/
InitDB.Initialize(app, app.Environment.IsProduction());

app.Run();