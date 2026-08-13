namespace OnlineAccountingApp.Application.Services.AppServices;

public interface ISeedService
{
    Task<SeedSampleDataResultDto> SeedSampleDataAsync(CancellationToken cancellationToken = default);
}
