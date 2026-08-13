using MediatR;
using OnlineAccountingApp.Application.Services.AppServices;

namespace OnlineAccountingApp.Application.Features.AppFeatures.SeedFeature.SeedSampleData;

public sealed class SeedSampleDataCommandHandler(ISeedService seedService)
    : IRequestHandler<SeedSampleDataCommand, SeedSampleDataResultDto>
{
    public Task<SeedSampleDataResultDto> Handle(SeedSampleDataCommand request, CancellationToken cancellationToken)
        => seedService.SeedSampleDataAsync(cancellationToken);
}
