namespace PolyStore.Domain.Exceptions;

//Hereda de Exception y le pasa el mensaje al padre
public class NotFoundException(string message) : Exception(message);