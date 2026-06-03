namespace PolyStore.Application.DTOs;

public class UserDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email {get; set; } = string.Empty;
    public string? FullName { get; set; } = string.Empty;
    public string? Token {get; set; } //Aqui ira el JWT mas adelante
    public string Role { get; set; } = "Customer";
}