namespace PolyStore.Application.Abstractions.Authentication;

public interface IUserContext
{
    string? UserId { get; }
    string? Role { get; } // Para decisiones de permisos
    bool IsAuthenticated { get; }
}