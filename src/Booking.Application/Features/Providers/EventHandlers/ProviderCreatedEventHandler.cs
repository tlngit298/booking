using Booking.Domain.Aggregates.Providers.Events;
using Microsoft.Extensions.Logging;

namespace Booking.Application.Features.Providers.EventHandlers;

/// <summary>
/// Handles the ProviderCreatedEvent by logging and performing side effects.
/// This handler demonstrates the event-driven architecture pattern.
/// </summary>
public sealed class ProviderCreatedEventHandler
    : INotificationHandler<ProviderCreatedEvent>
{
    private readonly ILogger<ProviderCreatedEventHandler> _logger;

    public ProviderCreatedEventHandler(
        ILogger<ProviderCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(
        ProviderCreatedEvent notification,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Provider created: {ProviderId} - {Name} ({Slug}) at {OccurredOn}",
            notification.ProviderId.Value,
            notification.Name,
            notification.Slug,
            notification.OccurredOn);

        await Task.CompletedTask;
    }
}
