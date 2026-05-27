using System;

namespace PolyStore.Application.Features.Orders.GetOrdersByUserId;

// Definimos el objeto DTO con los datos que permitimos que lleguen desde la Web
public record GetOrdersByUserIdRequest(Guid UserId);