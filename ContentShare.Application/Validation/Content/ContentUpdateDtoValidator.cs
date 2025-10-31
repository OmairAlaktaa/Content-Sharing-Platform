using ContentShare.Application.DTOs.Content;
using ContentShare.Domain.Enums;
using FluentValidation;

namespace ContentShare.Application.Validation.Content;

public class ContentUpdateDtoValidator : AbstractValidator<ContentUpdateDto>
{
    public ContentUpdateDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(1000);

        RuleFor(x => x.Category)
            .IsInEnum()
            .WithMessage($"Category must be one of: {string.Join(", ", Enum.GetNames(typeof(MediaCategory)).Select(n => n.ToLower()))}");

        RuleFor(x => x.ThumbnailUrl)
            .NotEmpty()
            .Must(BeValidHttpUrl).WithMessage("thumbnailUrl must be a valid http/https URL");

        RuleFor(x => x.ContentUrl)
            .NotEmpty()
            .Must(BeValidHttpUrl).WithMessage("contentUrl must be a valid http/https URL");
    }

    private static bool BeValidHttpUrl(string url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var u) &&
        (u.Scheme == Uri.UriSchemeHttp || u.Scheme == Uri.UriSchemeHttps);
}
