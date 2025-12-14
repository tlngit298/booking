namespace Booking.Application.Features.Providers.Commands.CreateProvider;

public sealed record CreateProviderCommand(
    string Name,
    string Slug,
    string Email,
    string TimeZone,
    string? Description = null,
    string? Phone = null) : IRequest<Result<Guid>>;
