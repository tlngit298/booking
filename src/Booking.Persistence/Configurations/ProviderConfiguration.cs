using Booking.Domain.Aggregates.Providers.ValueObjects;

namespace Booking.Persistence.Configurations;

public class ProviderConfiguration : IEntityTypeConfiguration<Provider>
{
    public void Configure(EntityTypeBuilder<Provider> builder)
    {
        // Table name
        builder.ToTable("Providers");

        // Primary key
        builder.HasKey(p => p.Id);

        // Configure ProviderId value object
        builder.Property(p => p.Id)
            .HasConversion(
                id => id.Value,           // To database: ProviderId → Guid
                value => new ProviderId(value))  // From database: Guid → ProviderId
            .ValueGeneratedNever();       // We generate IDs in domain (ProviderId.New())

        // Value object conversions for required fields
        builder.Property(p => p.Name)
            .HasConversion(
                name => name.Value,
                value => ProviderName.Create(value))
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Slug)
            .HasConversion(
                slug => slug.Value,
                value => Slug.Create(value))
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Email)
            .HasConversion(
                email => email.Value,
                value => Email.Create(value))
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.TimeZone)
            .HasConversion(
                tz => tz.Value,
                value => Booking.Domain.Aggregates.Providers.ValueObjects.TimeZone.Create(value))
            .IsRequired()
            .HasMaxLength(100);

        // Optional fields
        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.Phone)
            .HasMaxLength(50);

        // IsActive default value
        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Audit fields (CreatedAt, UpdatedAt from Entity<T> base class)
        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(p => p.Slug)
            .IsUnique()
            .HasDatabaseName("IX_Providers_Slug");

        builder.HasIndex(p => p.IsActive)
            .HasDatabaseName("IX_Providers_IsActive");
    }
}
