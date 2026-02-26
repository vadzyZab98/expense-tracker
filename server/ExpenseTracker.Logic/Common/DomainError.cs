namespace ExpenseTracker.Logic.Common;

public sealed record DomainError(ErrorCode Code, string Message)
{
    public static DomainError NotFound(string entity, object key)
        => new(ErrorCode.NotFound, $"{entity} with id '{key}' was not found.");

    public static DomainError Conflict(string message)
        => new(ErrorCode.Conflict, message);

    public static DomainError Unauthorized(string message)
        => new(ErrorCode.Unauthorized, message);
}
