using Invensa.Domain.Dtos;
using Invensa.Domain.Interfaces;
using Invensa.Domain.Models.Responses;
using LanguageExt.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Invensa.Application.Features.Clients.Queries;

namespace Invensa.Application.Features.Clients.Handlers;

public sealed class GetClientsQueryHandler : IRequestHandler<GetClientsQuery, Result<PaginatedResult<ClientDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetClientsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedResult<ClientDto>>> Handle(GetClientsQuery request, CancellationToken ct)
    {
        var result = await _unitOfWork.ClientRepository.GetByFiltersAsync(
            ct, request.Page, request.PageSize, request.SearchTerm, request.OnlyActive);

        return new Result<PaginatedResult<ClientDto>>(result);
    }
}
