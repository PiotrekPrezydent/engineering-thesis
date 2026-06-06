using Dara.BuildingBlocks.Domain.Models;

namespace Dara.Server.Modules.Identity.Domain.Clients;

public class ClientPlugin : ValueObject
{
    public string Name { get; }
    
    public IReadOnlyList<ClientPluginFunction> Functions { get; }
    
    private ClientPlugin(string name, IReadOnlyList<ClientPluginFunction> functions)
    {
        Name = name;
        Functions = functions;
    }

    public static ClientPlugin Create(string name, IReadOnlyList<ClientPluginFunction> functions)
    {
        return new ClientPlugin(name, functions);
    }
    
}