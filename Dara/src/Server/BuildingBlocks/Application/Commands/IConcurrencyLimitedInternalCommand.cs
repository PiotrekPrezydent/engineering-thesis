namespace Dara.Server.BuildingBlocks.Application.Commands;

// queued commands of one group share the limit set for the group in the processing options of the module
// (ConcurrencyLimits); a group without a limit is limited only by the parallelism of the module;
// mark the property with [JsonIgnore], so it is not stored in the command data
public interface IConcurrencyLimitedInternalCommand : ICommand
{
    string ConcurrencyGroup { get; }
}
