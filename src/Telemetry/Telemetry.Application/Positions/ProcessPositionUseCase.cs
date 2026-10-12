using FluentValidation;
using Telemetry.Application.Interfaces.Notifications;
using Telemetry.Application.Interfaces.Repositories;
using Telemetry.Domain.Entities;

namespace Telemetry.Application.Positions;

public class ProcessPositionUseCase(
    IValidator<PositionRequest> validator,
    IPositionRepository positionRepository,
    PositionAlarmCalculator alarmCalculator,
    IMonitoringNotifier monitoringNotifier)
{
    public async Task<PositionResponse> ExecuteAsync(PositionRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var alarms = alarmCalculator.Calculate(request);

        var position = new Position
        {
            VehicleId = request.VehicleId,
            Speed = request.Speed,
            Latitude = request.Latitude.Value,
            Longitude = request.Longitude.Value
        };

        var savedPosition = await positionRepository.AddAsync(position, cancellationToken);

        var positionProcessedEvent = new PositionProcessedEvent(
            PositionId: savedPosition.Id,
            VehicleId: savedPosition.VehicleId,
            Latitude: savedPosition.Latitude,
            Longitude: savedPosition.Longitude,
            Speed: request.Speed,
            Alarms: alarms);

        await monitoringNotifier.NotifyPositionProcessedAsync(positionProcessedEvent, cancellationToken);

        return new PositionResponse(
            PositionId: savedPosition.Id,
            VehicleId: savedPosition.VehicleId); ;
    }
}