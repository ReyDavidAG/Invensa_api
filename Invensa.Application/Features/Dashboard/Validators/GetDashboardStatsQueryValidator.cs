using FluentValidation;
using Invensa.Application.Features.Dashboard.Queries;

namespace Invensa.Application.Features.Dashboard.Validators;

public sealed class GetDashboardStatsQueryValidator : AbstractValidator<GetDashboardStatsQuery>
{
    public GetDashboardStatsQueryValidator()
    {
        RuleFor(x => x.ToUtc)
            .GreaterThanOrEqualTo(x => x.FromUtc)
            .When(x => x.FromUtc.HasValue && x.ToUtc.HasValue)
            .WithMessage("La fecha final no puede ser anterior a la fecha inicial.");
    }
}
