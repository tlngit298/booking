namespace Booking.Application.Features.Providers.Commands.DeactivateProvider;

public sealed class DeactivateProviderCommandHandler
    : IRequestHandler<DeactivateProviderCommand, Result>
{
    private readonly IProviderRepository _providerRepository;

    public DeactivateProviderCommandHandler(IProviderRepository providerRepository)
    {
        _providerRepository = providerRepository;
    }

    public async Task<Result> Handle(
        DeactivateProviderCommand request,
        CancellationToken cancellationToken)
    {
        var providerId = new ProviderId(request.ProviderId);
        var provider = await _providerRepository.GetByIdAsync(providerId, cancellationToken);

        if (provider is null)
        {
            return Result.Failure(
                Error.NotFound("Provider.NotFound", "Provider not found"));
        }

        provider.Deactivate();

        return Result.Success();
    }
}
