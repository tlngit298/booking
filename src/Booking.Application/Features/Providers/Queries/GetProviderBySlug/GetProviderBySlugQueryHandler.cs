namespace Booking.Application.Features.Providers.Queries.GetProviderBySlug;

public sealed class GetProviderBySlugQueryHandler
    : IRequestHandler<GetProviderBySlugQuery, Result<ProviderDetailsDto>>
{
    private readonly IProviderRepository _providerRepository;

    public GetProviderBySlugQueryHandler(IProviderRepository providerRepository)
    {
        _providerRepository = providerRepository;
    }

    public async Task<Result<ProviderDetailsDto>> Handle(
        GetProviderBySlugQuery request,
        CancellationToken cancellationToken)
    {
        var provider = await _providerRepository.GetBySlugAsync(request.Slug, cancellationToken);

        if (provider is null)
        {
            return Result<ProviderDetailsDto>.Failure(
                Error.NotFound("Provider.NotFound", "Provider not found"));
        }

        var dto = new ProviderDetailsDto(
            provider.Id.Value,
            provider.Name,
            provider.Slug,
            provider.Email,
            provider.Phone,
            provider.Description,
            provider.TimeZone,
            provider.IsActive,
            provider.CreatedAt,
            provider.UpdatedAt);

        return Result<ProviderDetailsDto>.Success(dto);
    }
}
