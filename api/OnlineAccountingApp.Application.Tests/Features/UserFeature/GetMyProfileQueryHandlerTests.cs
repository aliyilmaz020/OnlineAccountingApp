using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.UserFeature.GetMyProfile;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Domain.Entities.Identity;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Domain.Roles;

namespace OnlineAccountingApp.Application.Tests.Features.UserFeature;

public class GetMyProfileQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnCurrentUsersProfile()
    {
        Mock<IUserService> userService = new();
        Mock<ICompanyContext> companyContext = new();
        companyContext.Setup(c => c.UserId).Returns("user-1");
        companyContext.Setup(c => c.IsInRole(RoleList.SystemAdmin)).Returns(true);
        userService.Setup(s => s.GetByIdAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AppUser
            {
                Id = "user-1", UserName = "ahmet", Email = "ahmet@test.com", PhoneNumber = "5551234567",
                FirstName = "Ahmet", LastName = "Yılmaz"
            });

        GetMyProfileQueryHandler handler = new(userService.Object, companyContext.Object);

        MyProfileDto result = await handler.Handle(new GetMyProfileQuery(), CancellationToken.None);

        Assert.Equal("user-1", result.Id);
        Assert.Equal("ahmet", result.UserName);
        Assert.Equal("ahmet@test.com", result.Email);
        Assert.Equal("5551234567", result.PhoneNumber);
        Assert.Equal("Ahmet", result.FirstName);
        Assert.Equal("Yılmaz", result.LastName);
        Assert.True(result.IsAdmin);
    }

    [Fact]
    public async Task Handle_ShouldThrowBusinessException_WhenUserIsMissing()
    {
        Mock<IUserService> userService = new();
        Mock<ICompanyContext> companyContext = new();
        companyContext.Setup(c => c.UserId).Returns((string?)null);

        GetMyProfileQueryHandler handler = new(userService.Object, companyContext.Object);

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(new GetMyProfileQuery(), CancellationToken.None));
        Assert.Equal(AppErrorCodes.Auth.InvalidCredentials, exception.ErrorCode);
    }
}
