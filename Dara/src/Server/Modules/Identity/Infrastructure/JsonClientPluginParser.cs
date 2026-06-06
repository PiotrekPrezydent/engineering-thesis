using System.Text.Json;
using Dara.Server.Modules.Identity.Domain;
using Dara.Server.Modules.Identity.Domain.Clients;

namespace Dara.Server.Modules.Identity.Infrastructure;

public class JsonClientPluginParser : IClientPluginParser
{
        public bool CanHandle(string sourceType) => false;//sourceType.ToLower() == "json";

        public ClientPlugin Parse(string rawData) 
        {
            return JsonSerializer.Deserialize<ClientPlugin>(rawData);
        }
}