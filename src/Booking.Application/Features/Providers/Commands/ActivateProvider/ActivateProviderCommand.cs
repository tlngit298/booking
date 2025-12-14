namespace Booking.Application.Features.Providers.Commands.ActivateProvider;

public sealed record ActivateProviderCommand(Guid ProviderId) : IRequest<Result>;
