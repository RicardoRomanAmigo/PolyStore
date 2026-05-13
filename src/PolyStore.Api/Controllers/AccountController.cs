using Microsoft.AspNetCore.Mvc;
using PolyStore.Application.DTOs; 
using PolyStore.Application.Features.Authentication.Login;
using PolyStore.Application.Features.Authentication.Register;

namespace PolyStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly RegisterHandler _registerHandler; // <-- Agregamos las variables
    private readonly LoginHandler _loginHandler;    // <-- Agregamos las variables

    //Inyectamos ambos servicios en el constructor
    public AccountController(RegisterHandler registerHandler, LoginHandler loginHandler) // <-- Inyectamos los handlers
    {
       _registerHandler = registerHandler;
       _loginHandler = loginHandler;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterRequest request)
    {
        // El Handler valida, comprueba si existe, crea el usuario y genera el token.
        // Si algo falla, el ExceptionMiddleware captura la excepción.
        var result = await _registerHandler.HandleAsync(request);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginRequest request)
    {
        // El Handler valida formato, comprueba credenciales y genera el token.
        var result = await _loginHandler.HandleAsync(request);

        return Ok(result);
    }
}