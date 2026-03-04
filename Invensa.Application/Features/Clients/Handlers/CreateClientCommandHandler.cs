using Invensa.Application.Features.Clients.Validators;
using Invensa.Domain.Entities;
using Invensa.Domain.Exceptions;
using Invensa.Domain.Interfaces;
using LanguageExt.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Invensa.Application.Features.Clients.Commands;

public sealed class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateClientCommandHandler> _logger;

    public CreateClientCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateClientCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateClientCommand request, CancellationToken ct)
    {
        var validator = new CreateClientCommandValidator(_unitOfWork);
        var validationResult = await validator.ValidateAsync(request, ct);

        if (!validationResult.IsValid)
        {
            return new Result<Guid>(new FluentValidation.ValidationException(validationResult.Errors));
        }

        var client = new Client
        {
            Id = Guid.NewGuid(),
            Identifier = request.Identifier,
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            Active = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _unitOfWork.ClientRepository.AddAsync(client, ct);
        var saved = await _unitOfWork.SaveChangesAsync(ct);

        if (!saved)
        {
            return new Result<Guid>(new InfrastructureException("No se pudo guardar el cliente."));
        }

        return new Result<Guid>(client.Id);
    }
}
