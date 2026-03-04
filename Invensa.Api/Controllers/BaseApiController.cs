using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Invensa.Api.Controllers;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    private IMediator? _mediator;

    protected IMediator Mediator =>
        _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
}