using FluentValidation;
using FtpApi.Application.DTOs;

namespace FtpApi.Application.Validators;

public class UserRegisterValidator : AbstractValidator<UserRegisterDto>
{
    public UserRegisterValidator()
    {
        RuleFor(u => u.UserName)
            .NotEmpty().WithMessage("UserName is required.");

        RuleFor(u => u.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
            .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
            .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
            .Matches(@"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]+").WithMessage("Your password must contain at least one special character.");

        RuleFor(u => u.ConfirmedPassword)
            .Must((u, confirmedPass) => u.Password == confirmedPass).WithMessage("Passwords must match.");
    }
}
