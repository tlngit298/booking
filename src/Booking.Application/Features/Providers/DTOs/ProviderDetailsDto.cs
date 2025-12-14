namespace Booking.Application.Features.Providers.DTOs;

public sealed record ProviderDetailsDto(
    Guid Id,
    string Name,
    string Slug,
    string Email,
    string? Phone,
    string? Description,
    string TimeZone,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt);
