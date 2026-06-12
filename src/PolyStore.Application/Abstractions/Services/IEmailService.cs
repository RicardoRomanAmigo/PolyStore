namespace PolyStore.Application.Abstractions.Services;

public interface IEmailService
{
    Task SendOrderConfirmationAsync(Guid orderId, string email);
}