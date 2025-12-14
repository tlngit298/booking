namespace Booking.Application.Features.Providers.Queries.GetProviderById;

public sealed class GetProviderByIdQueryHandler
    : IRequestHandler<GetProviderByIdQuery, Result<ProviderDetailsDto>>
{
    private readonly IProviderRepository _providerRepository;

    public GetProviderByIdQueryHandler(IProviderRepository providerRepository)
    {
        _providerRepository = providerRepository;
    }

    public async Task<Result<ProviderDetailsDto>> Handle(
        GetProviderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var providerId = new ProviderId(request.ProviderId);
        var provider = await _providerRepository.GetByIdAsync(providerId, cancellationToken);

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
