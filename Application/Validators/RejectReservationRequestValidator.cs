using FluentValidation;
using MedicineFinder.Application.DTOs;

namespace MedicineFinder.Application.Validators;

public class RejectReservationRequestValidator : AbstractValidator<RejectReservationRequestDto>
{
    public RejectReservationRequestValidator()
    {
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}
