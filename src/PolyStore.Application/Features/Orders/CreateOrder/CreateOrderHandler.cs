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
    private readonly IUserAddressRepository _addressRepository; // //<--- Inyectamos el interface 
    private readonly IValidator<CreateOrderRequest> _validator;

    public CreateOrderHandler(
        IOrderRepository orderRepository, 
        IProductRepository productRepository,
        IUserAddressRepository addressRepository, //<---
        IValidator<CreateOrderRequest> validator)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _addressRepository = addressRepository; //<---
        _validator = validator;
    }

    public async Task<Guid> ExecuteAsync(CreateOrderRequest request)
    {
        // --- 1. VALIDACIÓN DE DATOS (FluentValidation) ---
        var validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key, 
                    g => g.Select(x => x.ErrorMessage).ToArray()
                );

            throw new PolyStore.Domain.Exceptions.ValidationException(errors);
        }

        // --- 2. LÓGICA DE NEGOCIO ---
        var domainItems = new List<OrderItem>();

        foreach (var itemDto in request.Items)
        {
            // Cambiado a tu método real: GetProductByIdAsync
            var product = await _productRepository.GetProductByIdAsync(itemDto.ProductId);
            
            if (product is null)
            {
                throw new Exception($"El producto con ID {itemDto.ProductId} no existe."); 
            }

            var orderItem = new OrderItem(product.Id, itemDto.Quantity, product.Price);
            domainItems.Add(orderItem);
        }

        var order = new Order(request.UserId, request.CustomerEmail, domainItems); // <---

        var shippingAddress = new UserAddress(   // <---
            request.UserId ?? Guid.Empty,
            request.Address.FullName,
            request.Address.Dni,
            request.Address.PhoneNumber,
            request.Address.Address,
            request.Address.City,
            request.Address.PostalCode
        );

        // --- 3. PERSISTENCIA ---
        // Usamos la nomenclatura que encaja con tu estilo (AddOrderAsync)
        await _orderRepository.AddOrderAsync(order);

        // Guardamos la dirección en su tabla correspondiente
        // Nota: Necesitarás inyectar tu StoreDbContext o un IRepository<UserAddress>
        // Si no tienes repositorio para UserAddress, lo ideal es usar el DbContext:
        await _addressRepository.AddAsync(shippingAddress); // <--- Usamos el repositorio, no el contexto
        
        // Confirmación de los cambios
        await _orderRepository.SaveChangesAsync();

        return order.Id;
    }
}