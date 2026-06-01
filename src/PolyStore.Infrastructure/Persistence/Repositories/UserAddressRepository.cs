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

    public async Task AddAsync(UserAddress address)
    {
        await _context.UserAddresses.AddAsync(address);
    }
}