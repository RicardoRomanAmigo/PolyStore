namespace PolyStore.Application.Features.Orders.CreateOrder;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PolyStore.Application.Abstractions.Persistence;
using PolyStore.Domain.Entities;
using PolyStore.Domain.Exceptions; 

public class CreateOrderHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUserAddressRepository _addressRepository; 
    private readonly IValidator<CreateOrderRequest> _validator;

    public CreateOrderHandler(
        IOrderRepository orderRepository, 
        IProductRepository productRepository,
        IUserAddressRepository addressRepository, 
        IValidator<CreateOrderRequest> validator)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _addressRepository = addressRepository; 
        _validator = validator;
    }

    public async Task<Guid> ExecuteAsync(CreateOrderRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray());

            throw new PolyStore.Domain.Exceptions.ValidationException(errors);
        }

        var domainItems = new List<OrderItem>();
        foreach (var itemDto in request.Items)
        {
            var product = await _productRepository.GetProductByIdAsync(itemDto.ProductId);
            if (product is null) throw new Exception($"El producto con ID {itemDto.ProductId} no existe."); 
            domainItems.Add(new OrderItem(product.Id, itemDto.Quantity, product.Price));
        }

        // 1. Crear el pedido
        var order = new Order(request.UserId, request.CustomerEmail, domainItems);
        
        // 2. Asignar la dirección al pedido (usando el nuevo método en la entidad)
        order.SetShippingAddress(
            request.Address.FullName,
            request.Address.Dni,
            request.Address.PhoneNumber,
            request.Address.Address,
            request.Address.City,
            request.Address.PostalCode
        );

        // 3. Persistencia
        await _orderRepository.AddOrderAsync(order);

        // 4. Solo guardamos en la tabla de direcciones de usuario SI el usuario existe
        if (request.UserId.HasValue) 
        {
            var shippingAddress = new UserAddress( 
                request.UserId.Value,
                request.Address.FullName,
                request.Address.Dni,
                request.Address.PhoneNumber,
                request.Address.Address,
                request.Address.City,
                request.Address.PostalCode
            );

            await _addressRepository.AddAsync(shippingAddress);
        }
        
        await _orderRepository.SaveChangesAsync();

        return order.Id;
    }
}