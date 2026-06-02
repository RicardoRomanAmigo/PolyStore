using Microsoft.EntityFrameworkCore;
using PolyStore.Application.Abstractions.Persistence;
using PolyStore.Domain.Entities;
using PolyStore.Infrastructure.Persistence.Context;

namespace PolyStore.Infrastructure.Persistence.Repositories;

public class UserAddressRepository : IUserAddressRepository
{
    private readonly StoreDbContext _context;
    
    public UserAddressRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<UserAddress?> GetByUserIdAsync(Guid userId)
    {
        // Buscamos la dirección asociada al UserId
        return await _context.UserAddresses
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task AddAsync(UserAddress address)
    {
        await _context.UserAddresses.AddAsync(address);
    }

    public async Task UpdateAsync(UserAddress address)
    {
        // En EF Core, al traer la entidad del contexto y modificar sus propiedades,
        // el cambio se rastrea automáticamente. Update() fuerza el marcado como modificado.
        _context.UserAddresses.Update(address);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}