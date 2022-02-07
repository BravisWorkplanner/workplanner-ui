namespace API;

public class Result<T>
{
    public string ErrorMessage { get; }

    public int StatusCode { get; }

    public T? Data { get; }

    public bool Success =>
        string.IsNullOrEmpty(ErrorMessage) && StatusCode is >= 200 and <= 299;

    public Result(T? data, int statusCode)
        : this(data, statusCode, "")
    {
    }

    public Result(int statusCode, string errorMessage)
        : this(default, statusCode, errorMessage)
    {
    }

    public Result(T? data, int statusCode, string errorMessage)
    {
        Data = data;
        StatusCode = statusCode;
        ErrorMessage = errorMessage;
    }
}