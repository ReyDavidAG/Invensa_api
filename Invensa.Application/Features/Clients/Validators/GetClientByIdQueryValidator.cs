using FluentValidation;
using Invensa.Application.Features.Clients.Queries;

namespace Invensa.Application.Features.Clients.Validators;

public sealed class GetClientByIdQueryValidator : AbstractValidator<GetClientByIdQuery>
{
    public GetClientByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID del cliente es obligatorio.");
    }
}
