using FluentValidation;
using Invensa.Application.Features.Clients.Commands;
using Invensa.Domain.Interfaces;

namespace Invensa.Application.Features.Clients.Validators;

public sealed class UpdateClientCommandValidator : AbstractValidator<UpdateClientCommand>
{
    public UpdateClientCommandValidator(IUnitOfWork uow)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID del cliente es obligatorio.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del cliente es obligatorio.")
            .MaximumLength(150).WithMessage("El nombre no puede exceder los 150 caracteres.");

        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("El formato del correo electrónico no es válido.")
            .CustomAsync(async (email, context, ct) =>
            {
                if (string.IsNullOrWhiteSpace(email)) return;
                var exists = await uow.ClientRepository.AnyAsync(c => c.Email == email && c.Id != context.InstanceToValidate.Id, ct);
                if (exists) context.AddFailure("Ya existe otro cliente registrado con este correo electrónico.");
            });

        RuleFor(x => x.Phone)
            .CustomAsync(async (phone, context, ct) =>
            {
                if (string.IsNullOrWhiteSpace(phone)) return;
                var exists = await uow.ClientRepository.AnyAsync(c => c.Phone == phone && c.Id != context.InstanceToValidate.Id, ct);
                if (exists) context.AddFailure("Ya existe otro cliente registrado con este número de teléfono.");
            });

        RuleFor(x => x.RowVersion)
            .NotEmpty().WithMessage("El RowVersion es obligatorio para el control de concurrencia.");

        RuleFor(x => x.Identifier)
            .MaximumLength(20).WithMessage("El identificador no puede exceder los 20 caracteres.")
            .CustomAsync(async (identifier, context, ct) =>
            {
                if (string.IsNullOrWhiteSpace(identifier)) return;

                // Valida que el identificador no lo tenga OTRO cliente distinto al que estamos editando
                var exists = await uow.ClientRepository.AnyAsync(c => c.Identifier == identifier && c.Id != context.InstanceToValidate.Id, ct);
                if (exists)
                {
                    context.AddFailure("Ya existe otro cliente registrado con este identificador.");
                }
            });
    }
}
