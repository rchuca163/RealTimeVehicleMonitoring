using Microsoft.EntityFrameworkCore;
using Monitoring.Api.Data;
using Monitoring.Api.Data.Repositories;
using Monitoring.Api.Hubs;
using Monitoring.Api.Services;
using Monitoring.Api.State;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();

builder.Services.AddDbContext<MonitoringDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TelemetryDb"))
           //.LogTo(Console.WriteLine, LogLevel.Information)
           );

builder.Services.AddScoped<MonitoringRepository>();
builder.Services.AddSingleton<VehicleStateStore>();
builder.Services.AddScoped<VehicleStateInitializer>();
builder.Services.AddScoped<MonitoringService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("SignalR", policy =>
    {
        policy
            .WithOrigins("http://127.0.0.1:5500",
                         "http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Inicialización del estado
using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<VehicleStateInitializer>();

    await initializer.InitializeAsync(CancellationToken.None);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapSwagger("/openapi/{documentName}.json");
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseCors("SignalR");

app.UseAuthorization();

app.MapControllers();

app.MapHub<MonitoringHub>("/hubs/monitoring");

app.Run();
