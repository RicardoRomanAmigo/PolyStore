namespace PolyStore.Domain.Entities;

public class UserAddress
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; } // FK hacia la clase User

    public string FullName { get; private set;} // por si el pedido lo recibe otra persona
    public string Dni { get; private set;}
    public string PhoneNumber { get; private set;}
    public string Address { get; private set;}
    public string City { get; private set;}
    public string PostalCode { get; private set;}
    public bool IsDefault { get; private set;}

    public UserAddress(Guid userId, string fullName, string dni, string phoneNumber, string address, string city, string postalCode, bool isDefault = true)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        FullName = fullName;
        Dni = dni;
        PhoneNumber = phoneNumber;
        Address = address;
        City = city;
        PostalCode = postalCode;
        IsDefault = isDefault;
    }
}