using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolyStore.Application.Abstractions.Authentication;
using PolyStore.Application.Abstractions.Persistence;
using PolyStore.Application.DTOs;
using PolyStore.Infrastructure.Persistence.Repositories;
using PolyStore.Domain.Entities;

namespace PolyStore.Api.Controllers;

[Authorize] // Proteccion para solo usuarios logueados
[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly IUserContext _userContext;
    private readonly IUserAddressRepository _addressRepository;

    public ProfileController(IUserContext userContext, IUserAddressRepository addressRepository)
    {
        _userContext = userContext;
        _addressRepository = addressRepository;
    }

    // GET: api/Profile/address
    [HttpGet("address")]
    public async Task<ActionResult<UserAddressDto>> GetAddress()
    {
        // Convertimos el string a Guid de forma segura
        if (!Guid.TryParse(_userContext.UserId, out Guid userId))
        {
            return BadRequest("El ID de usuario no es válido.");
        }

        var address = await _addressRepository.GetByUserIdAsync(userId);

        if (address == null)
            return NotFound("No se encontró una dirección guardada.");

        return Ok(new UserAddressDto
        {
            FullName = address.FullName,
            Dni = address.Dni,
            PhoneNumber = address.PhoneNumber,
            Address = address.Address,
            City = address.City,
            PostalCode = address.PostalCode
        });
    }

    // PUT: api/Profile/address
    [HttpPut("address")]
    public async Task<IActionResult> UpdateAddress([FromBody] UserAddressDto dto)
    {
        // Convertimos el string a Guid aquí mismo
        if (!Guid.TryParse(_userContext.UserId, out Guid userId))
        {
            return BadRequest("ID de usuario no válido.");
        }

        var address = await _addressRepository.GetByUserIdAsync(userId);

        if (address == null)
        {
            // Pasamos el userId (que ya es Guid) al constructor
            var newAddress = new UserAddress(userId, dto.FullName, dto.Dni, dto.PhoneNumber, dto.Address, dto.City, dto.PostalCode);
            await _addressRepository.AddAsync(newAddress);
        }
        else
        {
            address.Update(dto.FullName, dto.Dni, dto.PhoneNumber, dto.Address, dto.City, dto.PostalCode);
            await _addressRepository.UpdateAsync(address);
        }

        await _addressRepository.SaveChangesAsync();
        return NoContent();
    }
}
