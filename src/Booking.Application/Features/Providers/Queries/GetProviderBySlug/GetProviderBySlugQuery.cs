namespace Booking.Application.Features.Providers.Queries.GetProviderBySlug;

public sealed record GetProviderBySlugQuery(string Slug)
    : IRequest<Result<ProviderDetailsDto>>;
