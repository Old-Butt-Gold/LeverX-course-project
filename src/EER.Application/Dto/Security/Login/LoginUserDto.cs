﻿namespace EER.Application.Dto.Security.Login;

public class LoginUserDto
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}
