using Dara.Server.Modules.Identity.Domain.Clients;

namespace Dara.Server.Modules.Identity.Domain;

public interface IClientPluginParser
{
    bool CanHandle(string sourceType);
    
    ClientPlugin Parse(string sourceType);
}