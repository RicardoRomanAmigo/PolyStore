using PolyStore.Domain.Entities;

namespace PolyStore.Application.Abstractions.Authentication;

public interface IAuthService
{
    // Para registrar un nuevo usuario
    Task<User> Register(User user, string password);

    // Para verificar las credenciales y entrar
    Task<User?> Login(string email, string password);

    // Para evitar correos duplicados antes de intentar registar 
    Task<bool> UserExists(string email);
}