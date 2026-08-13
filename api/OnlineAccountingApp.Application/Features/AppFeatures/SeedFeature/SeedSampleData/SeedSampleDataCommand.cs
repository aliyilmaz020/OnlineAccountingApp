using MediatR;
using OnlineAccountingApp.Application.Services.AppServices;

namespace OnlineAccountingApp.Application.Features.AppFeatures.SeedFeature.SeedSampleData;

public sealed class SeedSampleDataCommand : IRequest<SeedSampleDataResultDto>
{
}
