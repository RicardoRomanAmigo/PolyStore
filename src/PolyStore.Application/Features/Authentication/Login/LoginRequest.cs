namespace PolyStore.Application.Features.Authentication.Login;

//Definimos el objeto DTO con los datos que permitimos que lleguen desde la web
public record LoginRequest(
    string Email,
    string Password
);