using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineAccountingApp.Application.Features.AppFeatures.UserFeature.ChangePassword;
using OnlineAccountingApp.Application.Features.AppFeatures.UserFeature.GetMyProfile;
using OnlineAccountingApp.Application.Features.AppFeatures.UserFeature.GetUsers;
using OnlineAccountingApp.Application.Features.AppFeatures.UserFeature.UpdateMyProfile;

namespace OnlineAccountingApp.WebApi.Controllers;

public class UsersController(IMediator mediator) : BaseApiController(mediator)
{
    [HttpGet("[action]")]
    public async Task<IActionResult> GetUsers([FromQuery] GetUsersQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetMyProfileQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPut("[action]")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateMyProfileCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
