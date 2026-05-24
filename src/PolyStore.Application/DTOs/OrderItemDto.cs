namespace PolyStore.Application.DTOs;

using System;

public class OrderItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}