using Microsoft.AspNetCore.SignalR;
using Monitoring.Api.Contracts;
using Monitoring.Api.Events;
using Monitoring.Api.Hubs;
using Monitoring.Api.State;

namespace Monitoring.Api.Services;

/// <summary>
/// Processes a vehicle position received from Telemetry, updates the vehicle's
/// current in-memory state, and notifies SignalR clients of the changes.
/// </summary>
/// <remarks>
/// If the vehicle is not present in the current monitoring state, the position
/// is ignored. For known vehicles, a position update is always published.
/// An alarm update is published only when the alarm state has changed.
/// </remarks>
public class MonitoringService(IHubContext<MonitoringHub> hubContext, VehicleStateStore stateStore)
{
    /// <summary>
    /// Processes the latest position received for a vehicle.
    /// </summary>
    /// <param name="position">
    /// Position event received from Telemetry.
    /// </param>
    public async Task ProcessPositionAsync(PositionEvent position)
    {
        if (!stateStore.TryGet(position.VehicleId, out var previousState))
            return;

        var vehicleGroup = $"vehicle:{position.VehicleId}";

        var alarmsChanged = !Equals(previousState!.Alarms, position.Alarms);

        var updatedState = new VehicleState(
            VehicleId: position.VehicleId,
            Plate: previousState.Plate,
            CompanyId: previousState.CompanyId,
            CompanyName: previousState.CompanyName,
            PositionId: position.PositionId,
            Latitude: position.Latitude,
            Longitude: position.Longitude,
            Speed: position.Speed,
            Alarms: position.Alarms);

        stateStore.Set(updatedState);

        var positionUpdatedEvent = new PositionUpdatedEvent(
            PositionId: position.PositionId,
            VehicleId: position.VehicleId,
            Latitude: position.Latitude,
            Longitude: position.Longitude,
            Speed: position.Speed);

        await hubContext.Clients
            .Group(vehicleGroup)
            .SendAsync("PositionUpdated", positionUpdatedEvent);

        if (!alarmsChanged)
            return;

        var alarmUpdatedEvent = new AlarmUpdatedEvent(
            VehicleId: position.VehicleId,
            Alarms: position.Alarms);

        await hubContext.Clients
            .Group(vehicleGroup)
            .SendAsync("AlarmUpdated", alarmUpdatedEvent);
    }

    public IReadOnlyCollection<VehicleSnapshotResponse> GetVehicleSnapshot(
        string? companyName,
        string? plate)
    {
        return stateStore.GetAll()
            .Values
            .Where(vehicle =>
                string.IsNullOrWhiteSpace(companyName) ||
                vehicle.CompanyName.Contains(companyName, StringComparison.OrdinalIgnoreCase))
            .Where(vehicle =>
                string.IsNullOrWhiteSpace(plate) ||
                vehicle.Plate.Contains(plate, StringComparison.OrdinalIgnoreCase))
            .Select(vehicle => new VehicleSnapshotResponse(
                vehicle.VehicleId,
                vehicle.CompanyId,
                vehicle.CompanyName,
                vehicle.Plate,
                vehicle.PositionId,
                vehicle.Latitude,
                vehicle.Longitude,
                vehicle.Speed,
                vehicle.Alarms))
            .ToList();
    }
}