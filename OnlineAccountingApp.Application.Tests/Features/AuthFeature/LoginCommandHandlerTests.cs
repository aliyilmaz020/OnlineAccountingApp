using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.AuthFeature.Login;
using OnlineAccountingApp.Application.Services.AppServices;

namespace OnlineAccountingApp.Application.Tests.Features.AuthFeature;

public class LoginCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldPassCredentialsThrough_AndReturnAuthServiceResult()
    {
        Mock<IAuthService> authService = new();
        AuthResponseDto expectedResponse = new()
        {
            AccessToken = "access-token",
            RefreshToken = "refresh-token",
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };

        authService.Setup(s => s.LoginAsync("user@example.com", "P@ssw0rd", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        LoginCommandHandler handler = new(authService.Object);
        LoginCommand command = new() { Email = "user@example.com", Password = "P@ssw0rd" };

        AuthResponseDto result = await handler.Handle(command, CancellationToken.None);

        Assert.Same(expectedResponse, result);
        authService.Verify(s => s.LoginAsync("user@example.com", "P@ssw0rd", It.IsAny<CancellationToken>()), Times.Once);
    }
}
