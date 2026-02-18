using FluentValidation;
using MedicineFinder.Application.DTOs;

namespace MedicineFinder.Application.Validators;

public class UpdateStockRequestValidator : AbstractValidator<UpdateStockRequestDto>
{
    public UpdateStockRequestValidator()
    {
        RuleFor(x => x.PharmacyId).NotEmpty();
        RuleFor(x => x.MedicineId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
    }
}
