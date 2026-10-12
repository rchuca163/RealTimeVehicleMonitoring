namespace Monitoring.Api.Events;

/// <summary>
/// Represents a GPS position update for an already monitored vehicle.
/// </summary>
public sealed record PositionUpdatedEvent(
    long PositionId,
    int VehicleId,
    decimal Latitude,
    decimal Longitude,
    decimal? Speed);