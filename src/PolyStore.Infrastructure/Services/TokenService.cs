using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PolyStore.Domain.Entities;
using PolyStore.Application.Abstractions.Authentication;

namespace PolyStore.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey _key;

    public TokenService(IConfiguration config)
    {
        //Leemos una clave secreta desde el appsettings.json
        var tokenKey = config["TokenKey"] ?? throw new Exception("TockenKey no encotrada.");
        _key  = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
    }

    public string CreateToken(User user)
    {
        // 1. Definimos los "Claims" ( La info que viaja dentro del Token)
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.NameId, user.UserName),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };
        
        // 2. Creamos las credenciales de firma (nuestra llave maestra)
        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

        // 3. Describimos como sera el token
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(7), // El token dura una semana
            SigningCredentials = creds
        };

        // 4. Creamos el manejador y generamos el token final
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}