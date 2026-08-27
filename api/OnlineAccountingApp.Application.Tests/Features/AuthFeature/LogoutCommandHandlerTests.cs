using MediatR;
using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.AuthFeature.Logout;
using OnlineAccountingApp.Application.Services.AppServices;

namespace OnlineAccountingApp.Application.Tests.Features.AuthFeature;

public class LogoutCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCallAuthServiceLogoutAsync()
    {
        Mock<IAuthService> authService = new();
        authService.Setup(s => s.LogoutAsync("old-refresh-token", It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        LogoutCommandHandler handler = new(authService.Object);
        LogoutCommand command = new() { RefreshToken = "old-refresh-token" };

        Unit result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(Unit.Value, result);
        authService.Verify(s => s.LogoutAsync("old-refresh-token", It.IsAny<CancellationToken>()), Times.Once);
    }
}
