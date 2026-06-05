namespace PolyStore.Application.Abstractions.Services;

public interface IOrderService
{
    Task CancelExpiredOrders();
}