namespace PolyStore.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string UserName { get; private set; }
    public string Email { get; private set; }

    // Contraseña en Hash
    public byte[] PasswordHash { get; private set; } = [];
    public byte[] PasswordSalt { get; private set; } = [];


    public string? FullName { get; private set; } 
    public string Role { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // --- Constructor para datos basicos---
    public User(string userName, string email, string? fullName = null, string role = "Customer")
    {
        Id = Guid.NewGuid();
        UserName = userName;
        Email = email.ToLower().Trim();
        FullName = fullName;
        Role = role;
        CreatedAt = DateTime.UtcNow;
    }

    // Metodo de dominio: El usuario no esta completo sin su seguridad
    public void SetPassword(byte[] hash, byte[] salt)
    {
        if(hash.Length == 0 || salt.Length == 0)
            throw new ArgumentException("El Hash y el Salt son obligatorios.");

            PasswordHash = hash;
            PasswordSalt = salt;
    }
}