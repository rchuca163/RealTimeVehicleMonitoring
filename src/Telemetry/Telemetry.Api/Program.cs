using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Telemetry.Api.Middleware;
using Telemetry.Application.Interfaces.Notifications;
using Telemetry.Application.Interfaces.Repositories;
using Telemetry.Application.Positions;
using Telemetry.Infrastructure.Data;
using Telemetry.Infrastructure.Data.Seed;
using Telemetry.Infrastructure.Notifications;
using Telemetry.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<TelemetryDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("TelemetryDb")));

builder.Services.AddHttpClient<IMonitoringNotifier, MonitoringNotifier>(
    client =>
    {
        client.BaseAddress = new Uri(
            builder.Configuration["Monitoring:BaseUrl"]!);
    });

builder.Services.AddScoped<ProcessPositionUseCase>();
builder.Services.AddScoped<IValidator<PositionRequest>, PositionRequestValidator>();
builder.Services.AddScoped<IPositionRepository, PositionRepository>();
builder.Services.AddScoped<PositionAlarmCalculator>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var context = scope.ServiceProvider
        .GetRequiredService<TelemetryDbContext>();

    await TelemetryDbSeeder.SeedAsync(context);

    //app.UseSwagger();
    //app.UseSwaggerUI();
    app.MapSwagger("/openapi/{documentName}.json");
    app.MapScalarApiReference();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
