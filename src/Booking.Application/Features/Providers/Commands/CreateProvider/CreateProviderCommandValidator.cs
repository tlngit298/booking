namespace Booking.Application.Features.Providers.Commands.CreateProvider;

public sealed class CreateProviderCommandValidator : AbstractValidator<CreateProviderCommand>
{
    public CreateProviderCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Provider name is required")
            .MaximumLength(200).WithMessage("Provider name must not exceed 200 characters");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug is required")
            .Matches("^[a-z0-9-]+$")
            .WithMessage("Slug must contain only lowercase letters, numbers, and hyphens")
            .MaximumLength(100).WithMessage("Slug must not exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email must not exceed 255 characters");

        RuleFor(x => x.TimeZone)
            .NotEmpty().WithMessage("TimeZone is required")
            .Must(BeValidTimeZone).WithMessage("Invalid IANA timezone");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("Phone must not exceed 20 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));
    }

    private bool BeValidTimeZone(string timeZone)
    {
        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
