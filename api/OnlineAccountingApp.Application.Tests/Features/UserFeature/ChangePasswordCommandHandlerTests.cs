using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.UserFeature.ChangePassword;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Domain.Exceptions;

namespace OnlineAccountingApp.Application.Tests.Features.UserFeature;

public class ChangePasswordCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldChangeCurrentUsersPassword_AndReturnTrue()
    {
        Mock<IUserService> userService = new();
        Mock<ICompanyContext> companyContext = new();
        companyContext.Setup(c => c.UserId).Returns("user-1");
        ChangePasswordCommand command = new() { CurrentPassword = "OldPass1!", NewPassword = "NewPass1!", ConfirmNewPassword = "NewPass1!" };

        ChangePasswordCommandHandler handler = new(userService.Object, companyContext.Object);

        bool result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
        userService.Verify(s => s.ChangePasswordAsync("user-1", "OldPass1!", "NewPass1!", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowBusinessException_WhenUserIsMissing()
    {
        Mock<IUserService> userService = new();
        Mock<ICompanyContext> companyContext = new();
        companyContext.Setup(c => c.UserId).Returns((string?)null);

        ChangePasswordCommandHandler handler = new(userService.Object, companyContext.Object);
        ChangePasswordCommand command = new() { CurrentPassword = "OldPass1!", NewPassword = "NewPass1!", ConfirmNewPassword = "NewPass1!" };

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));
        Assert.Equal(AppErrorCodes.Auth.InvalidCredentials, exception.ErrorCode);
    }
}
