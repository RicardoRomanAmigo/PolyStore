using Microsoft.AspNetCore.Http;
using PolyStore.Application.Abstractions.Authentication;
using System.Security.Claims;

namespace PolyStore.Infrastructure.Services;

public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{

    // El ID: Lo sacamos del claim NameIdentifier (el UUID configurado)							
    public string? UserId => httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    // EL ROL: Lo sacamos del claim Role que inyecta el TokenService							
    public string? Role => httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;

    // LA AUTENTICACIÓN: Verificamos si la identidad existe y si ha sido validada							
    public bool IsAuthenticated => httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}