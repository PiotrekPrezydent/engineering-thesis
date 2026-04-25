using Dara.BuildingBlocks.Application.Commands;

namespace Dara.BuildingBlocks.Application.Exceptions
{
    public class CommandResultStatusException : Exception
    {
        public CommandResultStatus CurrentStatus { get; private set; }
        public CommandResultStatus GivenStatus { get; private set; }
    
        public CommandResultStatusException(CommandResultStatus currentStatus, CommandResultStatus givenStatus, string message = "Command result status cannot be changed at this moment") : base(message)
        {
            CurrentStatus = currentStatus;
            GivenStatus = givenStatus;
        }
    }
}