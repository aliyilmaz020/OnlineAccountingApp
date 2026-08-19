using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.UserFeature.GetMyProfile;
using OnlineAccountingApp.Application.Features.AppFeatures.UserFeature.UpdateMyProfile;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Domain.Entities.Identity;
using OnlineAccountingApp.Domain.Exceptions;

namespace OnlineAccountingApp.Application.Tests.Features.UserFeature;

public class UpdateMyProfileCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldUpdateCurrentUsersProfile()
    {
        Mock<IUserService> userService = new();
        Mock<ICompanyContext> companyContext = new();
        companyContext.Setup(c => c.UserId).Returns("user-1");
        UpdateMyProfileCommand command = new()
        {
            UserName = "ayse", Email = "ayse@test.com", PhoneNumber = "5559876543", FirstName = "Ayşe", LastName = "Demir"
        };
        userService.Setup(s => s.UpdateProfileAsync(
                "user-1", "ayse", "ayse@test.com", "5559876543", "Ayşe", "Demir", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AppUser
            {
                Id = "user-1", UserName = "ayse", Email = "ayse@test.com", PhoneNumber = "5559876543",
                FirstName = "Ayşe", LastName = "Demir"
            });

        UpdateMyProfileCommandHandler handler = new(userService.Object, companyContext.Object);

        MyProfileDto result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal("ayse", result.UserName);
        Assert.Equal("ayse@test.com", result.Email);
        Assert.Equal("Ayşe", result.FirstName);
        Assert.Equal("Demir", result.LastName);
        userService.Verify(
            s => s.UpdateProfileAsync("user-1", "ayse", "ayse@test.com", "5559876543", "Ayşe", "Demir", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowBusinessException_WhenUserIsMissing()
    {
        Mock<IUserService> userService = new();
        Mock<ICompanyContext> companyContext = new();
        companyContext.Setup(c => c.UserId).Returns((string?)null);

        UpdateMyProfileCommandHandler handler = new(userService.Object, companyContext.Object);
        UpdateMyProfileCommand command = new() { UserName = "ayse", Email = "ayse@test.com" };

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));
        Assert.Equal(AppErrorCodes.Auth.InvalidCredentials, exception.ErrorCode);
    }
}
