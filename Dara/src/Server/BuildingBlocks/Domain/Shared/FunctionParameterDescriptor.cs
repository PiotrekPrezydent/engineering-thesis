namespace Dara.Server.BuildingBlocks.Domain.Shared;

public record FunctionParameterDescriptor(TypeDescriptor Type, string Name, string Description = "");