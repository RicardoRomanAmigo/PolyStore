namespace PolyStore.Application.Abstractions.Authentication;

public interface IUserContext
{
    string? UserId { get; }
    bool IsAuthenticated { get; }
}