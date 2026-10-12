using Telemetry.Application.Interfaces.Repositories;
using Telemetry.Domain.Entities;
using Telemetry.Infrastructure.Data;

namespace Telemetry.Infrastructure.Repositories;

public class PositionRepository(TelemetryDbContext context) : IPositionRepository
{
    public async Task<Position> AddAsync(Position position, CancellationToken cancellationToken)
    {
        context.Positions.Add(position);

        await context.SaveChangesAsync(cancellationToken);

        return position;
    }
}