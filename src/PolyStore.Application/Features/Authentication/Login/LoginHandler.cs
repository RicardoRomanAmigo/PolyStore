using FluentValidation;
using PolyStore.Application.Abstractions.Authentication;
using PolyStore.Application.DTOs;
using PolyStore.Domain.Entities;
// Alias para evitar ambigüedad con FluentValidation
using DomainExceptions = PolyStore.Domain.Exceptions;

namespace PolyStore.Application.Features.Authentication.Login;

public class LoginHandler
{
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;
    private readonly IValidator<LoginRequest> _validator;

    public LoginHandler(
        IAuthService authService,
        ITokenService tokenService,
        IValidator<LoginRequest> validator
    )
    {
        _authService = authService;
        _tokenService = tokenService;
        _validator = validator;
    }

    public async Task<UserDto> HandleAsync(LoginRequest request)
    {
        //1. Validar los datos con FluentValidation
        var validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            //Transformamos los errores al diccionario que espera el ValidationException
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                );
            throw new DomainExceptions.ValidationException(errors);
        }

        // 2. Intento de Login
        var user = await _authService.Login(request.Email.ToLower(), request.Password);

        // 3. Si falla, lanzamos error de validación (para que el Middleware lo pinte)
        if(user == null)
        {
            throw new DomainExceptions.ValidationException(new Dictionary<string, string[]>
            {
                {"Auth", new[] { "Email o contraseña inválidos."}}
            });
        }

        //4. Exito, devolvemos el dto con su token
        return new UserDto
        {
            UserName = user.UserName,
            Email = user.Email,
            Token = _tokenService.CreateToken(user),
            Role = user.Role
        };
    }
}