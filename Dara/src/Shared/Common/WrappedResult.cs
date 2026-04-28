namespace Dara.Shared.Common;

public class WrappedResult
{
    public Exception? Error
    {
        get
        {
            if (!IsSuccess)
                return _error;

            throw new ArgumentNullException(nameof(Error) +" is NULL");
        }
    }
    protected Exception? _error;
    
    public bool IsSuccess => Error == null;
    
    public WrappedResult() { } 
    
    public WrappedResult(Exception error) { _error = error; }
    
    public static implicit operator WrappedResult(Exception error) => new (error);
}

public class WrappedResult<T> : WrappedResult
{
    public T? Value
    {
        get
        {
            if (IsSuccess)
                return _value;
            
            throw new ArgumentNullException(nameof(Value) +" is NULL");
        }
    }
    private T? _value;
    
    //private set value
    WrappedResult(T value)
    {
        _value = value;
        _error = null;
    }
    
    //private set error
    WrappedResult(Exception error)
    {
        _value = default;
        _error = error;
    }
    
    public static implicit operator WrappedResult<T>(T value) => new (value);
    public static implicit operator WrappedResult<T>(Exception error) => new(error);
}