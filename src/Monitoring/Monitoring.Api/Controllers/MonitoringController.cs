using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Monitoring.Api.Contracts;
using Monitoring.Api.Hubs;
using Monitoring.Api.Services;
using Monitoring.Api.State;

namespace Monitoring.Api.Controllers;

/// <summary>
/// Provides endpoints for vehicle monitoring and real-time position updates.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MonitoringController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            Message = "Monitoring API funcionando"
        });
    }

    [HttpPost("test")]
    public async Task<IActionResult> SendTestMessage([FromServices] IHubContext<MonitoringHub> hubContext)
    {
        await hubContext.Clients.All.SendAsync("TestMessage", "¡Hola desde Monitoring API!");

        return Ok();
    }
    
    [HttpGet("vehicles")]
    public IActionResult GetVehicles(string? companyName, string? plate, [FromServices] MonitoringService monitoringService)
    {
        var snapshot = monitoringService.GetVehicleSnapshot(companyName, plate);

        return Ok(snapshot);
    }

    [HttpPost("positions")]
    public async Task<IActionResult> ReceivePosition(PositionEvent position, [FromServices] MonitoringService service)
    {
        await service.ProcessPositionAsync(position);

        return Ok();
    }
}