using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using Telemetry.Application.Interfaces.Notifications;
using Telemetry.Application.Positions;

namespace Telemetry.Infrastructure.Notifications
{
    public class MonitoringNotifier(HttpClient httpClient, ILogger<MonitoringNotifier> logger) : IMonitoringNotifier
    {
        public async Task NotifyPositionProcessedAsync(PositionProcessedEvent positionEvent, CancellationToken cancellationToken)
        {
            try
            {
                await httpClient.PostAsJsonAsync("api/Monitoring/positions", positionEvent, cancellationToken);
            }
            catch (HttpRequestException ex)
            {
                logger.LogWarning(ex, "Could not notify Monitoring for position {PositionId}.", positionEvent.PositionId);
            }
        }
    }
}
