namespace GeoGuard.Domain.Common;

public class Result<T>
{
    public T? Value { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public int StatusCode { get; }
    public string Error { get; } = string.Empty;
    private Result(bool isSuccess,T?value,int statusCode,string error)
    {
        IsSuccess = isSuccess;
        Value = value;
        StatusCode = statusCode;
        Error = error;
    }
    public static Result<T> Success(T value) => new(true, value, 200, string.Empty);
    public static Result<T> BadRequest(string error) => new(false, default, 400, error);
    public static Result<T> NotFound(string error) => new(false, default, 404, error);
    public static Result<T> ServerError(string error) => new(false, default, 500, error);
    public static Result<T> Conflict(string error) => new(false, default, 409, error);

}
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public int StatusCode { get; }
    public string Error { get; } = string.Empty;
    private Result(bool isSuccess, int statusCode, string error)
    {
        IsSuccess = isSuccess;
        StatusCode = statusCode;
        Error = error;
    }
    public static Result Success() => new(true, 200, string.Empty);
    public static Result BadRequest(string error) => new(false, 400, error);
    public static Result NotFound(string error) => new(false, 404, error);
    public static Result ServerError(string error) => new(false, 500, error);
    public static Result Conflict(string error) => new(false, 409, error);

}