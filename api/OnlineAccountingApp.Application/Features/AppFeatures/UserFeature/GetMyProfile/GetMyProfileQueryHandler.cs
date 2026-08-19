using Mapster;
using MediatR;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Domain.Entities.Identity;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Domain.Roles;

namespace OnlineAccountingApp.Application.Features.AppFeatures.UserFeature.GetMyProfile;

public sealed class GetMyProfileQueryHandler(IUserService userService, ICompanyContext companyContext)
    : IRequestHandler<GetMyProfileQuery, MyProfileDto>
{
    public async Task<MyProfileDto> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(companyContext.UserId))
        {
            throw new BusinessException(AppErrorCodes.Auth.InvalidCredentials, "Authentication is required to access this resource.");
        }

        AppUser user = await userService.GetByIdAsync(companyContext.UserId, cancellationToken);
        MyProfileDto dto = user.Adapt<MyProfileDto>();
        dto.IsAdmin = companyContext.IsInRole(RoleList.SystemAdmin);
        return dto;
    }
}
