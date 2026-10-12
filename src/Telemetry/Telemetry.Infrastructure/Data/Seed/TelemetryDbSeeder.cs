using Microsoft.EntityFrameworkCore;
using Telemetry.Domain.Entities;

namespace Telemetry.Infrastructure.Data.Seed;

public static class TelemetryDbSeeder
{
    public static async Task SeedAsync(TelemetryDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Companies.AnyAsync(cancellationToken))
            return;

        var companies = new[]
        {
            new Company { Name = "Empresa Norte" },
            new Company { Name = "Empresa Sur" }
        };

        await context.Companies.AddRangeAsync(companies, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var vehicles = new[]
        {
            new Vehicle
            {
                CompanyId = companies[0].Id,
                Plate = "AA123AA"
            },
            new Vehicle
            {
                CompanyId = companies[0].Id,
                Plate = "AA456AA"
            },
            new Vehicle
            {
                CompanyId = companies[0].Id,
                Plate = "AA789AA"
            },
            new Vehicle
            {
                CompanyId = companies[1].Id,
                Plate = "BB123BB"
            },
            new Vehicle
            {
                CompanyId = companies[1].Id,
                Plate = "BB456BB"
            },
            new Vehicle
            {
                CompanyId = companies[1].Id,
                Plate = "BB789BB"
            }
        };

        await context.Vehicles.AddRangeAsync(vehicles, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}