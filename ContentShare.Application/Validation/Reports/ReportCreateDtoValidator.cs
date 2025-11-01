using ContentShare.Application.DTOs.Reports;
using FluentValidation;

namespace ContentShare.Application.Validation.Reports;

public sealed class ReportCreateDtoValidator : AbstractValidator<ReportCreateDto>
{
    public ReportCreateDtoValidator()
    {
        RuleFor(x => x.Reason).IsInEnum();
        RuleFor(x => x.Note).MaximumLength(1000);
    }
}
