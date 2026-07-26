using MediatR;
using Microsoft.AspNetCore.Identity;
using OnlineAccountingApp.Application.Features.AppFeatures.AuthFeature.Login;
using OnlineAccountingApp.Application.Services;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Domain.Entities.Identity;
using OnlineAccountingApp.Domain.Exceptions;
using DomainValidationException = OnlineAccountingApp.Domain.Exceptions.ValidationException;
// The sibling AuthFeature.RefreshToken namespace otherwise shadows the entity type.
using RefreshTokenEntity = OnlineAccountingApp.Domain.Entities.RefreshToken;

namespace OnlineAccountingApp.Application.Features.AppFeatures.AuthFeature.Register;

public sealed class RegisterCommandHandler(
    UserManager<AppUser> userManager,
    ITokenService tokenService,
    IRefreshTokenService refreshTokenService,
    IUnitOfWork unitOfWork) : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        AppUser? existingUser = await userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            throw new BusinessException(AppErrorCodes.Auth.UserAlreadyExists, "A user with the same email already exists.");
        }

        var user = new AppUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = request.Email,
            Email = request.Email
        };

        IdentityResult result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors
                .GroupBy(error => error.Code)
                .ToDictionary(group => group.Key, group => group.Select(error => error.Description).ToArray());

            throw new DomainValidationException(errors);
        }

        IList<string> roles = await userManager.GetRolesAsync(user);
        AccessToken accessToken = tokenService.CreateAccessToken(user, roles);

        var refreshToken = new RefreshTokenEntity
        {
            Token = tokenService.CreateRefreshToken(),
            AppUserId = user.Id,
            ExpiresAt = tokenService.GetRefreshTokenExpiry(),
            Status = true
        };

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        await refreshTokenService.CreateAsync(refreshToken, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return new AuthResponseDto
        {
            AccessToken = accessToken.Token,
            RefreshToken = refreshToken.Token,
            AccessTokenExpiresAt = accessToken.ExpiresAt
        };
    }
}
