using Dara.BuildingBlocks.Domain.Models;

namespace Dara.Server.Modules.Identity.Domain.Clients;

public class ClientPluginFunction : ValueObject
{
    public string Name { get; }
    
    public string Description { get; }
    
    public IReadOnlyList<ClientPluginFunctionParameter> Parameters { get; }
    
    public Type? ReturnType { get; }
    
    private ClientPluginFunction(string name, string description, IReadOnlyList<ClientPluginFunctionParameter> parameters, Type? returnType = null)
    {
        Name = name;
        Description = description;
        Parameters = parameters;
        ReturnType = returnType;
    }

    public static ClientPluginFunction Create(string name, string description, IReadOnlyList<ClientPluginFunctionParameter> parameters,
        Type? returnType = null)
    {
        return new ClientPluginFunction(name, description, parameters, returnType);
    }
    
    
}