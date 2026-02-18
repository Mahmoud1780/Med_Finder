using FluentValidation;
using MedicineFinder.Application.DTOs;
using MedicineFinder.Domain.Enums;

namespace MedicineFinder.Application.Validators;

public class MedicineSearchRequestValidator : AbstractValidator<MedicineSearchRequestDto>
{
    public MedicineSearchRequestValidator()
    {
        RuleFor(x => x.Keyword).NotEmpty().MaximumLength(200);
        RuleFor(x => x.SortBy).IsInEnum();

        When(x => x.SortBy == SortBy.Nearest, () =>
        {
            RuleFor(x => x.Latitude).NotNull();
            RuleFor(x => x.Longitude).NotNull();
        });
    }
}
