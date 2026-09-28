using FluentValidation;
using EmployeeManagementPlatform.Api.DTOs;

namespace EmployeeManagementPlatform.Api.Validators;

public class CreateEmployeeDtoValidator : AbstractValidator<CreateEmployeeDto>
{
    public CreateEmployeeDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.")
            .Must((dto, lastName) => !string.Equals(dto.FirstName?.Trim(), lastName?.Trim(), StringComparison.OrdinalIgnoreCase))
            .WithMessage("First name and last name cannot be identical.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters.");

        RuleFor(x => x.Position)
            .NotEmpty().WithMessage("Position is required.")
            .MaximumLength(50).WithMessage("Position must not exceed 50 characters.");

        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Department is required.")
            .MaximumLength(50).WithMessage("Department must not exceed 50 characters.");

        RuleFor(x => x.Salary)
            .GreaterThanOrEqualTo(0).WithMessage("Salary must be non-negative.");

        RuleFor(x => x.HireDate)
            .NotEmpty().WithMessage("Hire date is required.")
            .GreaterThanOrEqualTo(new DateTime(1990, 1, 1)).WithMessage("Hire date cannot be earlier than 1990-01-01.")
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1)).WithMessage("Hire date cannot be in the future.");
    }
}

public class UpdateEmployeeDtoValidator : AbstractValidator<UpdateEmployeeDto>
{
    public UpdateEmployeeDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.")
            .Must((dto, lastName) => !string.Equals(dto.FirstName?.Trim(), lastName?.Trim(), StringComparison.OrdinalIgnoreCase))
            .WithMessage("First name and last name cannot be identical.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters.");

        RuleFor(x => x.Position)
            .NotEmpty().WithMessage("Position is required.")
            .MaximumLength(50).WithMessage("Position must not exceed 50 characters.");

        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Department is required.")
            .MaximumLength(50).WithMessage("Department must not exceed 50 characters.");

        RuleFor(x => x.Salary)
            .GreaterThanOrEqualTo(0).WithMessage("Salary must be non-negative.");

        RuleFor(x => x.HireDate)
            .NotEmpty().WithMessage("Hire date is required.")
            .GreaterThanOrEqualTo(new DateTime(1990, 1, 1)).WithMessage("Hire date cannot be earlier than 1990-01-01.")
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1)).WithMessage("Hire date cannot be in the future.");
    }
}
