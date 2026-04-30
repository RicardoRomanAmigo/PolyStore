using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PolyStore.Application.Abstractions.Authentication;
using PolyStore.Domain.Entities;
using PolyStore.Infrastructure.Persistence.Context;

namespace PolyStore.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly StoreDbContext _context;

    public AuthService(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<User> Register(User user, string password)
    {
        // 1.Creamos el Hash y el Salt
        using var hmac = new HMACSHA512();

        // Extraemos los valores
        var salt = hmac.Key;
        var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

        // Usamos el método de dominio (comportamiento)
        user.SetPassword(hash, salt);

        // 2. Guardamos en la DB
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User?> Login(string email, string password)
    {
        // 1. Buscamos el usuario por email
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null) return null; // No existe

        // 2. Verificamos contraseña
        using var hmac = new HMACSHA512(user.PasswordSalt); 
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        //Comparamos byte a byte el hash guardado con el que acabamos de calcular
        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i]) return null; // Contraseña erronea
        }

        return user; // exito en el login
    }

    public async Task<bool> UserExists(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }
}