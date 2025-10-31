using ContentShare.Application.DTOs.Rating;
using FluentValidation;

namespace ContentShare.Application.Validation.Rating;

public class RatingCreateDtoValidator : AbstractValidator<RatingCreateDto>
{
    public RatingCreateDtoValidator()
    {
        RuleFor(x => x.MediaContentId)
            .NotEmpty().WithMessage("MediaContentId is required.");

        RuleFor(x => x.Score)
            .InclusiveBetween(1, 5).WithMessage("Score must be between 1 and 5.");

        RuleFor(x => x.Comment)
            .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Comment));
    }
}
