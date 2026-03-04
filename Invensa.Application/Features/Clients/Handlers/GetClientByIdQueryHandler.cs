using Invensa.Domain.Dtos;
using Invensa.Domain.Exceptions;
using Invensa.Domain.Interfaces;
using LanguageExt.Common;
using MediatR;
using Invensa.Application.Features.Clients.Queries;

namespace Invensa.Application.Features.Clients.Handlers;

public sealed class GetClientByIdQueryHandler : IRequestHandler<GetClientByIdQuery, Result<ClientDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetClientByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ClientDto>> Handle(GetClientByIdQuery request, CancellationToken ct)
    {
        var dto = await _unitOfWork.ClientRepository.GetByIdAsync(ct, request.Id);

        if (dto == null)
        {
            return new Result<ClientDto>(new NotFoundException($"Cliente con ID {request.Id} no encontrado."));
        }

        return new Result<ClientDto>(dto);
    }
}
