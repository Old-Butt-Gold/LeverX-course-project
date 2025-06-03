using EER.Application.Abstractions.Security;
using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.Authentication.Commands.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, UserLoggedDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<UserLoggedDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var dto = request.LoginUserDto;

        var user = await _userRepository.GetByEmailAsync(dto.Email, cancellationToken: cancellationToken);

        if (user is null)
        {
            throw new KeyNotFoundException("User with provided email wasn't found");
        }

        var isIdentical = _passwordHasher.VerifyPassword(user.PasswordHash, dto.Password);

        var accessToken = isIdentical
            ? _jwtTokenService.GenerateAccessToken(user)
            : "";

        return new UserLoggedDto { AccessToken = accessToken, IsSuccess = isIdentical };
    }
}
