using Invensa.Application.Features.Clients.Commands;
using Invensa.Application.Features.Clients.Validators;
using Invensa.Domain.Exceptions;
using Invensa.Domain.Interfaces;
using LanguageExt.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Invensa.Application.Features.Clients.Handlers;

public sealed class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateClientCommandHandler> _logger;

    public UpdateClientCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateClientCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(UpdateClientCommand request, CancellationToken ct)
    {
        var validator = new UpdateClientCommandValidator(_unitOfWork);
        var validationResult = await validator.ValidateAsync(request, ct);

        if (!validationResult.IsValid)
        {
            return new Result<bool>(new FluentValidation.ValidationException(validationResult.Errors));
        }

        var client = await _unitOfWork.ClientRepository.FindAsync(ct, request.Id);

        if (client == null)
        {
            return new Result<bool>(new NotFoundException($"Cliente con ID {request.Id} no encontrado."));
        }

        // Control de concurrencia manual antes de persistir
        if (!client.RowVersion.SequenceEqual(request.RowVersion))
        {
            return new Result<bool>(new BadRequestException("El cliente fue modificado por otro usuario."));
        }

        // Configurar RowVersion original para EF
        _unitOfWork.SetOriginalRowVersion(client, request.RowVersion);

        // Actualizar propiedades
        client.Identifier = request.Identifier;
        client.Name = request.Name;
        client.Email = request.Email;
        client.Phone = request.Phone;
        client.Address = request.Address;
        client.Active = request.Active;
        client.UpdatedAtUtc = DateTime.UtcNow;

        try
        {
            var saved = await _unitOfWork.SaveChangesAsync(ct);
            if (!saved)
            {
                return new Result<bool>(new InfrastructureException("No se pudo actualizar el cliente. Es posible que no haya cambios para guardar."));
            }

            return new Result<bool>(true);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Conflicto de concurrencia al actualizar cliente {ClientId}", request.Id);
            return new Result<bool>(new BadRequestException("Los datos han sido modificados por otro usuario. Por favor, recargue e intente de nuevo."));
        }
    }
}
