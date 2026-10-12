namespace Telemetry.Application.Positions;

public record PositionRequest(
    int VehicleId,
    decimal? Latitude,
    decimal? Longitude,
    decimal Speed,
    bool PanicAlarm);
