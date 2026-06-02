using PolyStore.Domain.Entities;

namespace PolyStore.Application.Abstractions.Persistence;

public interface IUserAddressRepository
{
    Task<UserAddress?> GetByUserIdAsync(Guid userId);
    Task AddAsync(UserAddress address);
    Task UpdateAsync(UserAddress address);
    Task SaveChangesAsync();
}