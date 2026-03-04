using FluentValidation;
using Invensa.Application.Features.Units.Commands;
using Invensa.Domain.Interfaces;

namespace Invensa.Application.Features.Units.Validators;

public sealed class UpdateUnitCommandValidator : AbstractValidator<UpdateUnitCommand>
{
    public UpdateUnitCommandValidator(IUnitOfWork uow)
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().MaximumLength(40);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);

        RuleFor(x => x.Id)
            .MustAsync((id, ct) => uow.UnitRepository.AnyAsync(u => u.Id == id, ct))
            .WithMessage("La unidad no existe.");

        RuleFor(x => x)
            .MustAsync(async (cmd, ct) =>
                !await uow.UnitRepository.AnyAsync(u =>
                    u.Id != cmd.Id && (u.Code == cmd.Code || u.Name == cmd.Name), ct))
            .WithMessage("Code o Name ya están usados por otra unidad.");
    }
}
