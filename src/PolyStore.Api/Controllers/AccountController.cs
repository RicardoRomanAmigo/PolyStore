using Microsoft.AspNetCore.Mvc;
using PolyStore.Application.Abstractions.Authentication; // Para usar los interfaces de authentication
using PolyStore.Application.DTOs; //Para usar RegisterDto y UserDto
using PolyStore.Domain.Entities; // Para usar la entidad


namespace PolyStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;

    //Inyectamos ambos servicios en el constructor
    public AccountController(IAuthService authService, ITokenService tokenService)
    {
        _authService = authService;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        // 1. Validacion basica
        if(await _authService.UserExists(registerDto.Email))
            return BadRequest("Ese correo ya esta en uso.");
        
        // 2. Mapeo manual del DTO a la Entidad
        var user = new User(registerDto.UserName,registerDto.Email.ToLower(), registerDto.FullName);
       
        // 3. Guardar en DB  con el Token recien generado
        var createdUser = await _authService.Register(user, registerDto.Password);

        // 4. Devolver el DTO con el Token recien generado
        return new UserDto
        {
            UserName = createdUser.UserName,
            Email = createdUser.Email,
            Token = _tokenService.CreateToken(createdUser),
            Role = createdUser.Role
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        // 1. Llamamos al servicio de autenticacion
        var user = await _authService.Login(loginDto.Email.ToLower(),loginDto.Password);

        // 2. Si es null, las credenciales no coinciden
        if(user == null) return Unauthorized("Email o contraseña invalidos.");

        // 3. Si es correcto, devolvemos el UserDto con su pulsera (Token)
        return new UserDto
        {
            UserName = user.UserName,
            Email = user.Email,
            Token = _tokenService.CreateToken(user),
            Role = user.Role
        };
    }
}