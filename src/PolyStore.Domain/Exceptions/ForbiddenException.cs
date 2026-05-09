namespace PolyStore.Domain.Exceptions;

public class ForbiddenException(string message) : Exception(message);