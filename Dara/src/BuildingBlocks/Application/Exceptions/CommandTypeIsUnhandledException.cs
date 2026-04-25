using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.BuildingBlocks.Application.Exceptions;

public class CommandTypeIsUnhandledException : Exception
{
    public Type UsedCommand  { get; private set; }
    
    public Type Handler  { get; private set; }

    public CommandTypeIsUnhandledException(Type handler, Type usedCommand, string message = "Command is unhandled in this handler") : base(message)
    {
        UsedCommand = usedCommand;
        Handler = handler;
    }
}