using FluentValidation;
using PolyStore.Application.Abstractions.Authentication;
using PolyStore.Application.DTOs;
using PolyStore.Domain.Entities;
// Alias para evitar ambigüedad con FluentValidation
using DomainExceptions = PolyStore.Domain.Exceptions;

namespace PolyStore.Application.Features.Authentication.Register;

public class RegisterHandler
{
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;
    private readonly IValidator<RegisterRequest> _validator;

    public RegisterHandler(
        IAuthService authService, 
        ITokenService tokenService, 
        IValidator<RegisterRequest> validator)
    {
        _authService = authService;
        _tokenService = tokenService;
        _validator = validator;
    }

    public async Task<UserDto> HandleAsync(RegisterRequest request)
    {
        // 1. Validar los datos con FluentValidation
        var validationResult = await _validator.ValidateAsync(request);
        
        if (!validationResult.IsValid) 
        {
            // Transformamos los errores al diccionario que espera tu ValidationException
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key, 
                    g => g.Select(x => x.ErrorMessage).ToArray()
                );

            throw new DomainExceptions.ValidationException(errors);
        }

        // 2. Regla de negocio: No permitir duplicados
        if (await _authService.UserExists(request.Email))
        {
            throw new DomainExceptions.ValidationException(new Dictionary<string, string[]>
            {
                { "Email", new[] { "Este correo electrónico ya está en uso." } }
            });
        }

        // 3. Crear Entidad de Dominio usando TU constructor
        // Nota: Id se genera dentro y Role es "Customer" por defecto
        var user = new User(request.UserName, request.Email, request.FullName);

        // 4. Registro y Persistencia (Tu AuthService en Infrastructure)
        var registeredUser = await _authService.Register(user, request.Password);

        // 5. Devolvemos el UserDto con el Token generado por tu TokenService
        return new UserDto
        {
            UserName = registeredUser.UserName,
            Email = registeredUser.Email,
            Token = _tokenService.CreateToken(registeredUser),
            Role = registeredUser.Role
        };
    }
}