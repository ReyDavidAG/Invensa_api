using Invensa.Application.Features.Units.Commands;
using Invensa.Application.Features.Units.Validators;
using Invensa.Domain.Entities;
using Invensa.Domain.Exceptions;
using Invensa.Domain.Interfaces;
using LanguageExt.Common;
using MediatR;

namespace Invensa.Application.Features.Units.Handlers;

public sealed class CreateUnitCommandHandler
    : IRequestHandler<CreateUnitCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;

    public CreateUnitCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<Guid>> Handle(CreateUnitCommand request, CancellationToken ct)
    {
        var vr = await new CreateUnitCommandValidator(_uow).ValidateAsync(request, ct);
        if (!vr.IsValid) return new Result<Guid>(new FluentValidation.ValidationException(vr.Errors));

        var entity = new Domain.Entities.Unit
        {
            Id = Guid.NewGuid(),
            Code = request.Code.Trim().ToUpper(),
            Name = request.Name.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
            RowVersion = Array.Empty<byte>()
        };

        await _uow.UnitRepository.AddAsync(entity, ct);

        var saved = await _uow.SaveChangesAsync(ct);
        if (!saved) return new Result<Guid>(new InfrastructureException("No se pudo crear la unidad."));

        return new Result<Guid>(entity.Id);
    }
}
