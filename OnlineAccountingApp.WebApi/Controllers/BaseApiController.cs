using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace OnlineAccountingApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController(IMediator mediator) : ControllerBase
{
    protected readonly IMediator mediator = mediator;
}
