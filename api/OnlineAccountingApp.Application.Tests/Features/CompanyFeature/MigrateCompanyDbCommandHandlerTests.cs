using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.MigrateCompanyDb;
using OnlineAccountingApp.Application.Services.AppServices;

namespace OnlineAccountingApp.Application.Tests.Features.CompanyFeature;

public class MigrateCompanyDbCommandHandlerTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Handle_ShouldReturnServiceResult(bool serviceResult)
    {
        Mock<ICompanyService> companyService = new();
        companyService.Setup(s => s.MigrateCompanyDbAsync()).ReturnsAsync(serviceResult);

        MigrateCompanyDbCommandHandler handler = new(companyService.Object);

        bool result = await handler.Handle(new MigrateCompanyDbCommand(), CancellationToken.None);

        Assert.Equal(serviceResult, result);
        companyService.Verify(s => s.MigrateCompanyDbAsync(), Times.Once);
    }
}
