using Dara.BuildingBlocks.Domain.Models;

namespace Dara.Server.Modules.Identity.Domain.Clients;

public class ClientPluginFunctionParameter : ValueObject
{
    public string Name { get; }
    
    public string Description { get; }
    
    public Type Type { get; }
    
    public bool IsRequired { get; }

    private ClientPluginFunctionParameter(string name, string description, Type type, bool isRequired)
    {
        Name = name;
        Description = description;
        Type = type;
        IsRequired = isRequired;
    }

    public static ClientPluginFunctionParameter Create(string name, string description, Type type, bool isRequired)
    {
        return new ClientPluginFunctionParameter(name, description, type, isRequired);
    }
}