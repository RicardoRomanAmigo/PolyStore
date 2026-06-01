using PolyStore.Domain.Entities;

namespace PolyStore.Application.Abstractions.Persistence;

public interface IUserAddressRepository
{
    Task AddAsync(UserAddress address);
}