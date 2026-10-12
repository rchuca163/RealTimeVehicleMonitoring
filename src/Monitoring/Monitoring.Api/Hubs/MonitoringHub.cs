using Microsoft.AspNetCore.SignalR;

namespace Monitoring.Api.Hubs;

public class MonitoringHub : Hub
{
    public async Task SendTestMessage(string message)
    {
        await Clients.All.SendAsync("TestMessage", message);
    }

    public async Task JoinVehicleGroup(int vehicleId)
    {
        var vehicleGroup = $"vehicle:{vehicleId}";

        await Groups.AddToGroupAsync(Context.ConnectionId, vehicleGroup);
    }

    public async Task LeaveVehicleGroup(int vehicleId)
    {
        var vehicleGroup = $"vehicle:{vehicleId}";

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, vehicleGroup);
    }
}