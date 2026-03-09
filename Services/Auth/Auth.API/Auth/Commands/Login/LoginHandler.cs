namespace Auth.API.Auth.Commands.Login;

public class LoginHandler(IPasswordHasher passwordHasher, ITokenService tokenService, 
    IOptions<JwtOptions> options, IUsersRepository repo) : ICommandHandler<LoginCommand, LoginResult>
{
    private JwtOptions _options => options.Value;
    public async Task<LoginResult> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var user = await repo.GetUserByEmailAsync(command.Email, cancellationToken)??
            throw new NotFoundException("User", command.Email);

        if (!passwordHasher.VerifyPassword(user.PasswordHash, command.Password))
            throw new BadRequestException("Invalid password");
        
        var tokens = tokenService.GenerateTokens(user.Id);
        
        tokenService.SetTokenWithId(user.Id, _options.TokenName, _options.AccessTokenExpirationMinutes);
        
        user.RefreshToken = tokens.RefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_options.RefreshTokenExpirationDays);
        await repo.UpdateAsync(user, cancellationToken); 

        return new LoginResult(user.Id, tokens.RefreshToken);
    }
}