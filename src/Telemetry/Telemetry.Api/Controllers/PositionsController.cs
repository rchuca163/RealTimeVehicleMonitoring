using Microsoft.AspNetCore.Mvc;
using Telemetry.Application.Positions;

namespace Telemetry.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PositionsController(ProcessPositionUseCase receivePositionUseCase) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<PositionResponse>> Receive(PositionRequest request, CancellationToken cancellationToken)
        {
            var response = await receivePositionUseCase.ExecuteAsync(request, cancellationToken);

            return Ok(response);
        }

        [HttpPost("batch")]
        public async Task<ActionResult<IReadOnlyCollection<PositionResponse>>> ReceiveBatch(
            [FromQuery] int vehicleId,
            BatchPositionRequest request,
            CancellationToken cancellationToken)
        {
            var responses = new List<PositionResponse>();

            foreach (var position in request.Positions)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);

                var positionRequest = new PositionRequest(
                    VehicleId: vehicleId,
                    Latitude: position.Latitude,
                    Longitude: position.Longitude,
                    Speed: position.Speed,
                    PanicAlarm: position.PanicAlarm);

                var response = await receivePositionUseCase.ExecuteAsync(positionRequest, cancellationToken);

                responses.Add(response);
            }

            return Ok(responses);
        }
    }
}
