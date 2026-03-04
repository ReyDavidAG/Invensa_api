using FluentValidation;
using Invensa.Application.Features.Clients.Commands;
using Invensa.Domain.Interfaces;

namespace Invensa.Application.Features.Clients.Validators;

public sealed class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientCommandValidator(IUnitOfWork uow)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del cliente es obligatorio.")
            .MaximumLength(150).WithMessage("El nombre no puede exceder los 150 caracteres.");

        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("El formato del correo electrónico no es válido.")
            .MustAsync(async (email, ct) => 
            {
                if (string.IsNullOrWhiteSpace(email)) return true;
                return !await uow.ClientRepository.AnyAsync(c => c.Email == email, ct);
            }).WithMessage("Ya existe un cliente registrado con este correo electrónico.");

        RuleFor(x => x.Phone)
            .MustAsync(async (phone, ct) =>
            {
                if (string.IsNullOrWhiteSpace(phone)) return true;
                return !await uow.ClientRepository.AnyAsync(c => c.Phone == phone, ct);
            }).WithMessage("Ya existe un cliente registrado con este número de teléfono.");

        RuleFor(x => x.Identifier)
            .MaximumLength(20).WithMessage("El identificador no puede exceder los 20 caracteres.")
            .CustomAsync(async (identifier, context, ct) =>
            {
                if (string.IsNullOrWhiteSpace(identifier)) return;

                var exists = await uow.ClientRepository.AnyAsync(c => c.Identifier == identifier, ct);
                if (exists)
                {
                    context.AddFailure("Ya existe un cliente registrado con este identificador.");
                }
            });
    }
}
