namespace Booking.Application.Common.Behaviors;

/// <summary>
/// Pipeline behavior that automatically saves changes and publishes domain events for commands.
/// Only applies to requests whose name ends with "Command".
/// Ensures events are published AFTER successful persistence (within same transaction boundary).
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public class UnitOfWorkBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublisher _publisher;

    public UnitOfWorkBehavior(
        IUnitOfWork unitOfWork,
        IPublisher publisher)
    {
        _unitOfWork = unitOfWork;
        _publisher = publisher;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Only apply to commands (not queries)
        if (!typeof(TRequest).Name.EndsWith("Command"))
        {
            return await next();
        }

        var response = await next();

        var domainEvents = _unitOfWork.GetDomainEvents();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }

        _unitOfWork.ClearDomainEvents();

        return response;
    }
}
