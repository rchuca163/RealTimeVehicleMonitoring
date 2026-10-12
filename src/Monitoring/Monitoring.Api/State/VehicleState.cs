namespace Monitoring.Api.State;

/// <summary>
/// Represents the current monitoring state of a vehicle.
/// </summary>
/// <remarks>
/// The state can be partially reconstructed when Monitoring starts because the
/// persisted position does not contain speed or alarm information. In that case,
/// those properties are represented as null because their values are unknown.
/// </remarks>
public record VehicleState(
    int VehicleId,
    string Plate,
    int CompanyId,
    string CompanyName,
    long? PositionId,
    decimal? Latitude,
    decimal? Longitude,
    decimal? Speed,
    PositionAlarms? Alarms);