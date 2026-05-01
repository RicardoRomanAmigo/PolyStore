
using PolyStore.Domain.Entities;

namespace PolyStore.Application.Abstractions.Authentication;

public interface ITokenService
{
    string CreateToken(User user);
}