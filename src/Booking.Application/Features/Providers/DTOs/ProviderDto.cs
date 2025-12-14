namespace Booking.Application.Features.Providers.DTOs;

public sealed record ProviderDto(
    Guid Id,
    string Name,
    string Slug,
    string Email,
    string TimeZone,
    bool IsActive);
