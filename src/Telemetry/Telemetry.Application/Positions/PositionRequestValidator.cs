using FluentValidation;

namespace Telemetry.Application.Positions;

public class PositionRequestValidator : AbstractValidator<PositionRequest>
{
    public PositionRequestValidator()
    {
        RuleFor(x => x.VehicleId)
            .GreaterThan(0);

        RuleFor(x => x.Latitude)
            .NotNull()
            .InclusiveBetween(-90, 90);

        RuleFor(x => x.Longitude)
            .NotNull()
            .InclusiveBetween(-180, 180);

        RuleFor(x => x.Speed)
            .InclusiveBetween(0, 400);
    }
}