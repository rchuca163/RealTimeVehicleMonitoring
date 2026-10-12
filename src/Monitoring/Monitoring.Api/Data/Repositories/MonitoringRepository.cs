using Microsoft.EntityFrameworkCore;
using Monitoring.Api.Data.Models;
using System.Linq;

namespace Monitoring.Api.Data.Repositories;

public class MonitoringRepository(MonitoringDbContext context)
{
    /// <summary>
    /// Retrieves all vehicles together with their latest persisted position, when available.
    /// </summary>
    /// <remarks>
    /// A left join is used intentionally so vehicles without a persisted position are also
    /// returned. Those vehicles are represented with a null position and can therefore be
    /// initialized in the monitoring state without requiring an initial telemetry event.
    /// </remarks>
    public async Task<IReadOnlyCollection<InitialVehicleState>> GetInitialVehicleStatesAsync(CancellationToken cancellationToken)
    {
        return await context.Vehicles
            .Select(vehicle => new InitialVehicleState(
                vehicle.Id,
                vehicle.Plate,
                vehicle.CompanyId,
                vehicle.Company.Name,
                context.Positions
                    .Where(position => position.VehicleId == vehicle.Id)
                    .OrderByDescending(position => position.Id)
                    .Select(position => (long?) position.Id)
                    .FirstOrDefault(),
                context.Positions
                    .Where(position => position.VehicleId == vehicle.Id)
                    .OrderByDescending(position => position.Id)
                    .Select(position => (decimal?) position.Latitude)
                    .FirstOrDefault(),
                context.Positions
                    .Where(position => position.VehicleId == vehicle.Id)
                    .OrderByDescending(position => position.Id)
                    .Select(position => (decimal?) position.Longitude)
                    .FirstOrDefault(),
                context.Positions
                    .Where(position => position.VehicleId == vehicle.Id)
                    .OrderByDescending(position => position.Id)
                    .Select(position => (decimal?) position.Speed)
                    .FirstOrDefault()))
            .ToListAsync(cancellationToken);
    }
}