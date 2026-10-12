namespace Telemetry.Application.Positions;

public record PositionAlarms(
    bool Panic,
    bool Speeding,
    bool Stopped);