using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.Modules.Identity.Application.GetClient;
using Dara.Server.Modules.Identity.Domain;

namespace Dara.Server.Modules.Identity.Application;

public interface IClientQueries : IQueryHelper
{
    public Task<Client?> GetClientByIdentifierAsync(string clientIdentifier);
}