using ContentShare.Application.DTOs.Common;
using ContentShare.Domain.Enums;
using FluentValidation;

namespace ContentShare.Application.Validation.Common;

public class PagedRequestValidator : AbstractValidator<PagedRequest>
{
    private static readonly string[] AllowedSort =
        { "title", "-title", "created_at", "-created_at" };

    public PagedRequestValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);

        RuleFor(x => x.Sort)
            .Must(s => s is null || AllowedSort.Contains(s.ToLower()))
            .WithMessage($"Sort must be one of: {string.Join(", ", AllowedSort)}");

        RuleFor(x => x.Category)
            .Must(BeValidCategory).WithMessage("Category must be one of: game, video, artwork, music")
            .When(x => !string.IsNullOrWhiteSpace(x.Category));
    }

    private static bool BeValidCategory(string? value)
        => Enum.TryParse<MediaCategory>(value, true, out _);
}
