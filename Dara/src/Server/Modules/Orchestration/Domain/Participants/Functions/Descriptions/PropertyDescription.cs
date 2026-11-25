using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Orchestration.Domain.Participants.Functions.Descriptions;

// a function parameter or an object property
public record PropertyDescription(string Name, TypeDescription Type, bool Required) : IValueObject;
