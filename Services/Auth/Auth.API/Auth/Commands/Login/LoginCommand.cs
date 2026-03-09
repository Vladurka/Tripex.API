

namespace Auth.API.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : ICommand<LoginResult>;
public record LoginResult(Guid UserId, string RefreshToken);

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}