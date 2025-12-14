namespace Booking.Application.Features.Providers.Commands.UpdateProvider;

public sealed record UpdateProviderCommand(
    Guid ProviderId,
    string Name,
    string Email,
    string? Description = null,
    string? Phone = null) : IRequest<Result>;
