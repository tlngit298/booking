namespace Booking.Application.Features.Providers.Commands.CreateProvider;

public sealed class CreateProviderCommandHandler
    : IRequestHandler<CreateProviderCommand, Result<Guid>>
{
    private readonly IProviderRepository _providerRepository;

    public CreateProviderCommandHandler(
        IProviderRepository providerRepository)
    {
        _providerRepository = providerRepository;
    }

    public async Task<Result<Guid>> Handle(
        CreateProviderCommand request,
        CancellationToken cancellationToken)
    {
        // Check slug uniqueness
        if (await _providerRepository.SlugExistsAsync(request.Slug, cancellationToken))
        {
            return Result<Guid>.Failure(
                Error.Conflict("Provider.SlugExists", "A provider with this slug already exists"));
        }

        // Create domain entity with all fields at once
        var provider = Provider.Create(
            request.Name,
            request.Slug,
            request.Email,
            request.TimeZone,
            request.Description,
            request.Phone);

        // Persist
        await _providerRepository.AddAsync(provider, cancellationToken);

        return Result<Guid>.Success(provider.Id.Value);
    }
}
