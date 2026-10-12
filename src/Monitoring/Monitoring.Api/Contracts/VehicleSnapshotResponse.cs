using Monitoring.Api.State;

namespace Monitoring.Api.Contracts
{
    public record VehicleSnapshotResponse(
        int VehicleId,
        int CompanyId,
        string CompanyName,
        string Plate,
        long? PositionId,
        decimal? Latitude,
        decimal? Longitude,
        decimal? Speed,
        PositionAlarms? Alarms);
}
