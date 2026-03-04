using Invensa.Application.Features.Units.Commands;
using Invensa.Application.Features.Units.Validators;
using Invensa.Domain.Entities;
using Invensa.Domain.Exceptions;
using Invensa.Domain.Interfaces;
using LanguageExt.Common;
using MediatR;

namespace Invensa.Application.Features.Units.Handlers;

public sealed class UpdateUnitCommandHandler
    : IRequestHandler<UpdateUnitCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;

    public UpdateUnitCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<bool>> Handle(UpdateUnitCommand request, CancellationToken ct)
    {
        var vr = await new UpdateUnitCommandValidator(_uow).ValidateAsync(request, ct);
        if (!vr.IsValid) return new Result<bool>(new FluentValidation.ValidationException(vr.Errors));

        var entity = await _uow.UnitRepository.FindAsync(ct, request.Id);
        if (entity is null) return new Result<bool>(new NotFoundException("Unidad no encontrada."));

        entity.Code = request.Code.Trim().ToUpper();
        entity.Name = request.Name.Trim();
        entity.UpdatedAtUtc = DateTime.UtcNow;

        _uow.UnitRepository.Update(entity);

        var saved = await _uow.SaveChangesAsync(ct);
        if (!saved) return new Result<bool>(new InfrastructureException("No se pudo actualizar la unidad."));

        return new Result<bool>(true);
    }
}
