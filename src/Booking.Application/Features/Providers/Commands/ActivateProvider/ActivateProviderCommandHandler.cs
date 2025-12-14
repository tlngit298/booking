namespace Booking.Application.Features.Providers.Commands.ActivateProvider;

public sealed class ActivateProviderCommandHandler
    : IRequestHandler<ActivateProviderCommand, Result>
{
    private readonly IProviderRepository _providerRepository;

    public ActivateProviderCommandHandler(IProviderRepository providerRepository)
    {
        _providerRepository = providerRepository;
    }

    public async Task<Result> Handle(
        ActivateProviderCommand request,
        CancellationToken cancellationToken)
    {
        var providerId = new ProviderId(request.ProviderId);
        var provider = await _providerRepository.GetByIdAsync(providerId, cancellationToken);

        if (provider is null)
        {
            return Result.Failure(
                Error.NotFound("Provider.NotFound", "Provider not found"));
        }

        provider.Activate();

        return Result.Success();
    }
}
