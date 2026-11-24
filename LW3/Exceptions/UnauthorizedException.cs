namespace LW3.Exceptions;

// 401: облікові дані або refresh token недійсні
public class UnauthorizedException : BusinessException
{
    public UnauthorizedException(string message) : base(message) { }
    public override int StatusCode => StatusCodes.Status401Unauthorized;
    public override string Title => "Unauthorized";
}
