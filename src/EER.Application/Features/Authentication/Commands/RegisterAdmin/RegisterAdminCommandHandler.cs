using AutoMapper;
using EER.Application.Abstractions.Security;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.Authentication.Commands.RegisterAdmin;

public class RegisterAdminCommandHandler : IRequestHandler<RegisterAdminCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;

    public RegisterAdminCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
    }

    public async Task Handle(RegisterAdminCommand request, CancellationToken cancellationToken)
    {
        var dto = request.AdminDto;

        var admin = _mapper.Map<User>(dto);

        admin.PasswordHash = _passwordHasher.HashPassword(dto.Password);

        await _userRepository.AddAsync(admin, cancellationToken: cancellationToken);
    }
}
