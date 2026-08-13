using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.AuthFeature.Register;
using OnlineAccountingApp.Application.Services.AppServices;

namespace OnlineAccountingApp.Application.Tests.Features.AuthFeature;

public class RegisterCommandHandlerTests
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

        authService.Setup(s => s.RegisterAsync("new-user@example.com", "P@ssw0rd", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        RegisterCommandHandler handler = new(authService.Object);
        RegisterCommand command = new() { Email = "new-user@example.com", Password = "P@ssw0rd" };

        AuthResponseDto result = await handler.Handle(command, CancellationToken.None);

        Assert.Same(expectedResponse, result);
        authService.Verify(s => s.RegisterAsync("new-user@example.com", "P@ssw0rd", It.IsAny<CancellationToken>()), Times.Once);
    }
}
