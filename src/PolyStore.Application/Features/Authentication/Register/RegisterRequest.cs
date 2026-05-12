namespace PolyStore.Application.Features.Authentication.Register;

//Definimos el objeto DTO con los datos que permitimos que lleguen desde la web
public record RegisterRequest(
    string UserName,
    string Email,
    string Password,
    string? FullName = null
);