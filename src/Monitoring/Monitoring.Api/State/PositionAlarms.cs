namespace Monitoring.Api.State;

public record PositionAlarms(
    bool Panic,
    bool Speeding,
    bool Stopped);