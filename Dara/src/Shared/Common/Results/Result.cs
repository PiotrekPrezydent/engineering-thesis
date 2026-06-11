namespace Dara.Shared.Common.Results;

public record Result
{
    public bool IsSuccess { get; }
    public Error? Error { get; }

    protected Result()
    {
        IsSuccess = true;
        Error = null;
    }
    
    protected Result(Error error)
    {
        IsSuccess = false;
        Error = error;
    }

    public static Result Success() => new();
    public static Result Failure(Error error) => new(error ?? throw new ArgumentNullException(nameof(error)));
}

public record Result<T> : Result
{
    public T? Value { get; }

    protected Result(T value)
    {
        Value = value;
    }

    protected Result(Error error) : base(error)
    {
        Value = default;
    }
    
    public static Result<T> Success(T value) => new(value);

    public new static Result<T> Failure(Error error) => new(error ?? throw new ArgumentNullException(nameof(error)));

}