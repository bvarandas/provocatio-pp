namespace Challenge.Domain.Models.Results;

public class Result
{
    public bool IsSuccess { get; }
    public string Error { get; }
    public bool IsFailure => !IsSuccess;

    protected Result(bool isSuccess, string error)
    {
        if (isSuccess && !string.IsNullOrEmpty(error))
            throw new InvalidOperationException();
        if (!isSuccess && string.IsNullOrEmpty(error))
            throw new InvalidOperationException();

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Ok() => new Result(true, string.Empty);
    public static Result Success() => new Result(true, string.Empty);
    public static Result Fail(string message) => new Result(false, message);
}

public class Result<T> : Result
{
    private readonly T _value;

    // The value can only be accessed if the result is successful.
    public T Value
    {
        get
        {
            if (!IsSuccess)
                throw new InvalidOperationException("Cannot access value on a failed result.");
            return _value;
        }
    }

    protected Result(T value, bool isSuccess, string error) : base(isSuccess, error)
    {
        _value = value;
    }

    public static Result<T> Ok(T value) => new Result<T>(value, true, string.Empty);
    public static Result<T> Success(T value) => new Result<T>(value, true, string.Empty);
    public static Result<T> Fail(string message) => new Result<T>(default(T), false, message);

}