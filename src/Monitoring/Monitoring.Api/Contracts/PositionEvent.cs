using Monitoring.Api.State;

namespace Monitoring.Api.Contracts;

public sealed record PositionEvent(
    long PositionId,
    int VehicleId,
    decimal Latitude,
    decimal Longitude,
    decimal Speed,
    PositionAlarms Alarms);