using Monitoring.Api.State;

namespace Monitoring.Api.Events;

/// <summary>
/// Represents a change in the alarm state of an already monitored vehicle.
/// </summary>
public sealed record AlarmUpdatedEvent(
    int VehicleId,
    PositionAlarms? Alarms);