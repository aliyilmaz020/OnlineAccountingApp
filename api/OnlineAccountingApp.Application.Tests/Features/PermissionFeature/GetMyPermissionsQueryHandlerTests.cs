using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.PermissionFeature.GetMyPermissions;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Domain.Exceptions;

namespace OnlineAccountingApp.Application.Tests.Features.PermissionFeature;

public class GetMyPermissionsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnPermissionCodes_ForCurrentUserAndCompany()
    {
        Mock<IPermissionService> permissionService = new();
        Mock<ICompanyContext> companyContext = new();
        companyContext.Setup(c => c.CompanyId).Returns("company-1");
        companyContext.Setup(c => c.UserId).Returns("user-1");
        permissionService
            .Setup(s => s.GetPermissionCodesAsync("user-1", "company-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<string>)["UCAF.Read", "UCAF.Create"]);

        GetMyPermissionsQueryHandler handler = new(permissionService.Object, companyContext.Object);

        List<string> result = await handler.Handle(new GetMyPermissionsQuery(), CancellationToken.None);

        Assert.Equal(["UCAF.Read", "UCAF.Create"], result);
        permissionService.Verify(s => s.GetPermissionCodesAsync("user-1", "company-1", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowBusinessException_WhenCompanyHeaderIsMissing()
    {
        Mock<IPermissionService> permissionService = new();
        Mock<ICompanyContext> companyContext = new();
        companyContext.Setup(c => c.CompanyId).Returns((string?)null);
        companyContext.Setup(c => c.UserId).Returns("user-1");

        GetMyPermissionsQueryHandler handler = new(permissionService.Object, companyContext.Object);

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(new GetMyPermissionsQuery(), CancellationToken.None));
        Assert.Equal(AppErrorCodes.Tenant.CompanyNotSpecified, exception.ErrorCode);
    }

    [Fact]
    public async Task Handle_ShouldThrowBusinessException_WhenUserIsMissing()
    {
        Mock<IPermissionService> permissionService = new();
        Mock<ICompanyContext> companyContext = new();
        companyContext.Setup(c => c.CompanyId).Returns("company-1");
        companyContext.Setup(c => c.UserId).Returns((string?)null);

        GetMyPermissionsQueryHandler handler = new(permissionService.Object, companyContext.Object);

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(new GetMyPermissionsQuery(), CancellationToken.None));
        Assert.Equal(AppErrorCodes.Auth.InvalidCredentials, exception.ErrorCode);
    }
}
