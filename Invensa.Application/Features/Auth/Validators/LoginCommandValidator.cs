using FluentValidation;
using Invensa.Application.Features.Auth.Commands;
using Invensa.Domain.Interfaces;

namespace Invensa.Application.Features.Auth.Validators;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator(IUnitOfWork unitOfWork)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido")
            .EmailAddress().WithMessage("El email debe ser válido")
            .MustAsync(async (email, ct) => await unitOfWork.UserRepository.AnyAsync(x => x.Email == email, ct))
            .WithMessage("El correo electronico no existe.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida");
    }
}
