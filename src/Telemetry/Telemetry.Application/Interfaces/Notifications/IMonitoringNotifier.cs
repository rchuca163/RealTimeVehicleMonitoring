using Telemetry.Application.Positions;

namespace Telemetry.Application.Interfaces.Notifications;

public interface IMonitoringNotifier
{
    Task NotifyPositionProcessedAsync(PositionProcessedEvent positionEvent, CancellationToken cancellationToken);
}