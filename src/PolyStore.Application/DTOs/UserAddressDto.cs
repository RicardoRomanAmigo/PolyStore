namespace PolyStore.Application.DTOs;

public class UserAddressDto
{
    public string FullName {get; set; } = string.Empty;
    public string Dni { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
}
