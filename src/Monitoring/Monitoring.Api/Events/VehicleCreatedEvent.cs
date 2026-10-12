using Monitoring.Api.State;

namespace Monitoring.Api.Events;

/// <summary>
/// Represents the complete initial state of a vehicle detected by Monitoring.
/// This event allows connected clients to start monitoring the vehicle immediately
/// without requiring additional HTTP requests.
/// </summary>
public sealed record VehicleCreatedEvent(
    int VehicleId,
    int CompanyId,
    string Plate,
    long PositionId,
    decimal Latitude,
    decimal Longitude,
    decimal? Speed,
    PositionAlarms? Alarms);