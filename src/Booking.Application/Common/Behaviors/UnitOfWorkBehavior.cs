namespace Booking.Application.Common.Behaviors;

/// <summary>
/// Pipeline behavior that automatically saves changes for commands.
/// Only applies to requests whose name ends with "Command".
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public class UnitOfWorkBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public UnitOfWorkBehavior(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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

        // Execute handler
        var response = await next();

        // Save changes if command succeeded
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return response;
    }
}
