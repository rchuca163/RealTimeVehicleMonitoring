using Telemetry.Domain.Entities;

namespace Telemetry.Application.Interfaces.Repositories
{
    public interface IPositionRepository
    {
        Task<Position> AddAsync(Position position, CancellationToken cancellationToken);
    }
}
