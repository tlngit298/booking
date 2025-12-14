namespace Booking.Persistence.Repositories;

public class ProviderRepository : IProviderRepository
{
    private readonly ApplicationDbContext _context;

    public ProviderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Provider?> GetByIdAsync(
        ProviderId id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Providers
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Provider?> GetBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        return await _context.Providers
            .FirstOrDefaultAsync(p => p.Slug == slug, cancellationToken);
    }

    public async Task<bool> SlugExistsAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        return await _context.Providers
            .AnyAsync(p => p.Slug == slug, cancellationToken);
    }

    public async Task AddAsync(
        Provider provider,
        CancellationToken cancellationToken = default)
    {
        await _context.Providers.AddAsync(provider, cancellationToken);
    }

    public Task UpdateAsync(
        Provider provider,
        CancellationToken cancellationToken = default)
    {
        _context.Providers.Update(provider);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(
        ProviderId id,
        CancellationToken cancellationToken = default)
    {
        var provider = await _context.Providers
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (provider is not null)
        {
            _context.Providers.Remove(provider);
        }
    }
}
