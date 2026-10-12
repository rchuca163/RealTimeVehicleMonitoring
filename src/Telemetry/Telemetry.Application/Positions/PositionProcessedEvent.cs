namespace Telemetry.Application.Positions;

public record PositionProcessedEvent(
    long PositionId,
    int VehicleId,
    decimal Latitude,
    decimal Longitude,
    decimal Speed,
    PositionAlarms Alarms);