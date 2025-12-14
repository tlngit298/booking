namespace Booking.Application.Features.Providers.Commands.UpdateProvider;

public sealed class UpdateProviderCommandValidator : AbstractValidator<UpdateProviderCommand>
{
    public UpdateProviderCommandValidator()
    {
        RuleFor(x => x.ProviderId)
            .NotEmpty().WithMessage("Provider ID is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Provider name is required")
            .MaximumLength(200).WithMessage("Provider name must not exceed 200 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email must not exceed 255 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("Phone must not exceed 20 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));
    }
}
