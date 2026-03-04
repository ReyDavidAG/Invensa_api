namespace Invensa.Application.Features.Users.Validators
{
    using Domain.Interfaces;
    using FluentValidation;
    using Queries;

    public class GetSelfUserQueryValidator : AbstractValidator<GetSelfUserQuery>
    {
        public GetSelfUserQueryValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(r => r.Id)
                .NotEmpty()
                .WithMessage("El Id del usuario no puede ser vacío.")
                .MustAsync(async (id, cancellation) =>
                {
                    if (!Guid.TryParse(id, out var guid))
                        return false;
                    return await unitOfWork.UserRepository.AnyAsync(u => u.Id == guid, cancellation);
                })
                .WithMessage("El usuario solicitante no existe.");
        }
    }
}
