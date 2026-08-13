using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.AuthFeature.RefreshToken;
using OnlineAccountingApp.Application.Services.AppServices;

namespace OnlineAccountingApp.Application.Tests.Features.AuthFeature;

public class RefreshTokenCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldPassRefreshTokenThrough_AndReturnAuthServiceResult()
    {
        Mock<IAuthService> authService = new();
        AuthResponseDto expectedResponse = new()
        {
            AccessToken = "new-access-token",
            RefreshToken = "new-refresh-token",
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };

        authService.Setup(s => s.RefreshTokenAsync("old-refresh-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        RefreshTokenCommandHandler handler = new(authService.Object);
        RefreshTokenCommand command = new() { RefreshToken = "old-refresh-token" };

        AuthResponseDto result = await handler.Handle(command, CancellationToken.None);

        Assert.Same(expectedResponse, result);
        authService.Verify(s => s.RefreshTokenAsync("old-refresh-token", It.IsAny<CancellationToken>()), Times.Once);
    }
}
