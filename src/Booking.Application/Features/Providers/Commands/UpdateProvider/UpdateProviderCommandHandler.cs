namespace Booking.Application.Features.Providers.Commands.UpdateProvider;

public sealed class UpdateProviderCommandHandler
    : IRequestHandler<UpdateProviderCommand, Result>
{
    private readonly IProviderRepository _providerRepository;

    public UpdateProviderCommandHandler(IProviderRepository providerRepository)
    {
        _providerRepository = providerRepository;
    }

    public async Task<Result> Handle(
        UpdateProviderCommand request,
        CancellationToken cancellationToken)
    {
        var providerId = new ProviderId(request.ProviderId);
        var provider = await _providerRepository.GetByIdAsync(providerId, cancellationToken);

        if (provider is null)
        {
            return Result.Failure(
                Error.NotFound("Provider.NotFound", "Provider not found"));
        }

        provider.Update(
            request.Name,
            request.Description,
            request.Email,
            request.Phone);

        return Result.Success();
    }
}
