namespace Telemetry.Application.Positions;

public class PositionAlarmCalculator
{
    private const decimal SpeedingThreshold = 80;

    public PositionAlarms Calculate(PositionRequest request)
    {
        return new PositionAlarms(
            Panic:  request.PanicAlarm,
            Speeding: request.Speed >= SpeedingThreshold,
            Stopped: request.Speed == 0);
    }
}