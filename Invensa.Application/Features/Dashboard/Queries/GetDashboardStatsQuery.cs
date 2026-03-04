using Invensa.Domain.Dtos;
using LanguageExt.Common;
using MediatR;

namespace Invensa.Application.Features.Dashboard.Queries;

public sealed class GetDashboardStatsQuery : IRequest<Result<DashboardStatsDto>>
{
    public DateTime? FromUtc { get; init; }
    public DateTime? ToUtc { get; init; }
}
