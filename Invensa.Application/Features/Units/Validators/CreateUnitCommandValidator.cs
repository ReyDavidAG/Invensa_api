using FluentValidation;
using Invensa.Application.Features.Units.Commands;
using Invensa.Domain.Interfaces;

namespace Invensa.Application.Features.Units.Validators;

public sealed class CreateUnitCommandValidator : AbstractValidator<CreateUnitCommand>
{
    public CreateUnitCommandValidator(IUnitOfWork uow)
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(40);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);

        RuleFor(x => x.Code)
            .MustAsync((code, ct) => uow.UnitRepository.AnyAsync(u => u.Code == code, ct).ContinueWith(t => !t.Result))
            .WithMessage("Ya existe una unidad con ese código.");

        RuleFor(x => x.Name)
            .MustAsync((name, ct) => uow.UnitRepository.AnyAsync(u => u.Name == name, ct).ContinueWith(t => !t.Result))
            .WithMessage("Ya existe una unidad con ese nombre.");
    }
}
