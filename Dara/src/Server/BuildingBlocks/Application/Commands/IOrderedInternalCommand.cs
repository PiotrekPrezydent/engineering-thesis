namespace Dara.Server.BuildingBlocks.Application.Commands;

// queued commands with the same key run one after another in the order they were queued;
// mark the property with [JsonIgnore], so it is not stored in the command data
public interface IOrderedInternalCommand : ICommand
{
    string OrderingKey { get; }
}
