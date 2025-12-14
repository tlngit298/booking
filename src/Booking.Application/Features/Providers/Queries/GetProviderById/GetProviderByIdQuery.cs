namespace Booking.Application.Features.Providers.Queries.GetProviderById;

public sealed record GetProviderByIdQuery(Guid ProviderId)
    : IRequest<Result<ProviderDetailsDto>>;
