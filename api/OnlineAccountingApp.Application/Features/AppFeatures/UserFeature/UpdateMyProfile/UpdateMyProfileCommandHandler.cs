using Mapster;
using MediatR;
using OnlineAccountingApp.Application.Features.AppFeatures.UserFeature.GetMyProfile;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Domain.Entities.Identity;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Domain.Roles;

namespace OnlineAccountingApp.Application.Features.AppFeatures.UserFeature.UpdateMyProfile;

public sealed class UpdateMyProfileCommandHandler(IUserService userService, ICompanyContext companyContext)
    : IRequestHandler<UpdateMyProfileCommand, MyProfileDto>
{
    public async Task<MyProfileDto> Handle(UpdateMyProfileCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(companyContext.UserId))
        {
            throw new BusinessException(AppErrorCodes.Auth.InvalidCredentials, "Authentication is required to access this resource.");
        }

        AppUser user = await userService.UpdateProfileAsync(
            companyContext.UserId, request.UserName, request.Email, request.PhoneNumber,
            request.FirstName, request.LastName, cancellationToken);

        MyProfileDto dto = user.Adapt<MyProfileDto>();
        dto.IsAdmin = companyContext.IsInRole(RoleList.SystemAdmin);
        return dto;
    }
}
