namespace Booking.Application.Features.Providers.Commands.DeactivateProvider;

public sealed record DeactivateProviderCommand(Guid ProviderId) : IRequest<Result>;
