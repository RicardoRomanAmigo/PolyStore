namespace PolyStore.Domain.Exceptions;

public class ValidationException : Exception
{
    //Diccionario donde la llave es el campo (pj. "Price")
    //y el valor es el error (pj. "No puede ser negativo")
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors) 
    : base("Se han producido uno o más errores de validacion.") 
    {
        Errors = errors;
    }
}