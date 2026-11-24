namespace LW3.Exceptions;

// Єдиний підхід до помилок у застосунку: сервіси кидають бізнес-винятки,
// а ExceptionHandlingMiddleware перетворює їх на HTTP-відповідь.
public abstract class BusinessException : Exception
{
    protected BusinessException(string message) : base(message)
    {
    }

    public abstract int StatusCode { get; }
    public abstract string Title { get; }
}

// 404: потрібної сутності не існує
public class NotFoundException : BusinessException
{
    public NotFoundException(string message) : base(message) { }
    public override int StatusCode => StatusCodes.Status404NotFound;
    public override string Title => "Not Found";
}

// 400: вхідні дані некоректні
public class BadRequestException : BusinessException
{
    public BadRequestException(string message) : base(message) { }
    public override int StatusCode => StatusCodes.Status400BadRequest;
    public override string Title => "Bad Request";
}

// 409: операція суперечить поточному стану сутності або бізнес-правилу
public class ConflictException : BusinessException
{
    public ConflictException(string message) : base(message) { }
    public override int StatusCode => StatusCodes.Status409Conflict;
    public override string Title => "Conflict";
}
