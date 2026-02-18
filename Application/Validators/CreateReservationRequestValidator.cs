using FluentValidation;
using MedicineFinder.Application.DTOs;

namespace MedicineFinder.Application.Validators;

public class CreateReservationRequestValidator : AbstractValidator<CreateReservationRequestDto>
{
    public CreateReservationRequestValidator()
    {
        RuleFor(x => x.PharmacyId).NotEmpty();
        RuleFor(x => x.MedicineId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
