using Dara.BuildingBlocks.Application.Abstraction;
using Dara.BuildingBlocks.Application.Exceptions;

namespace Dara.BuildingBlocks.Application;

public class CommandResult : IApplicationCommand
{
    public IApplicationCommandResult? ExpectedResult { get; private set; }

    public CommandResultStatus Status { get; private set; }

    public Exception? ResultedException { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public CommandResult()
    {
        Status = CommandResultStatus.Pending;
        
        ResultedException = null;
        ExpectedResult = null;
        
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetExpectedResult(IApplicationCommandResult expectedResult)
    {
        if(Status != CommandResultStatus.Pending)
            throw new CommandResultStatusException(Status, CommandResultStatus.Success);
        
        if(ResultedException != null)
            throw new ArgumentException("Cannot set result after exception is already set", nameof(ResultedException));
        
        if(ExpectedResult != null)
            throw new ArgumentException("ExpectedResult is already set", nameof(expectedResult));
        
        Status = CommandResultStatus.Success;
        ExpectedResult = expectedResult;
        
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void SetException(Exception ex)
    {
        if(Status != CommandResultStatus.Pending)
            throw new CommandResultStatusException(Status, CommandResultStatus.Exception);
        
        if(ExpectedResult != null)
            throw new ArgumentException("Cannot set exception after result is already set", nameof(ExpectedResult));
        
        if(ResultedException != null)
            throw new ArgumentException("Exception is already set", nameof(ex));
        
        Status = CommandResultStatus.Failure;
        ResultedException = ex;
        
        UpdatedAt = DateTime.UtcNow;
    }

    public T GetResult<T>() where T : IApplicationCommandResult
    {
        if (Status != CommandResultStatus.Success)
            throw new ArgumentException("Cannot take result after failing");
        
        if(ExpectedResult == null)
            throw new NullReferenceException("ExpectedResult is null");
        
        return (T)ExpectedResult;
    }
}