namespace Telemetry.Application.Positions
{
    public record BatchPositionRequest(IReadOnlyCollection<BatchPosition> Positions);

    public record BatchPosition(
        decimal Latitude,
        decimal Longitude,
        decimal Speed,
        bool PanicAlarm);


}
