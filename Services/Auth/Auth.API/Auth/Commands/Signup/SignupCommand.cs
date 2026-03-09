namespace Auth.API.Auth.Commands.Signup;

public record SignupCommand(string UserName, string Email,
    string Password, string ConfirmPassword) : ICommand<RegisterResult>;

public record RegisterResult(Guid UserId, string RefreshToken);

public class RegisterCommandValidator : AbstractValidator<SignupCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.UserName).NotEmpty().MinimumLength(3).MaximumLength(25);
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password cannot be empty")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("Password must contain at least one number")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
        RuleFor(x => x.ConfirmPassword).Equal(x => x.Password);
    }
}