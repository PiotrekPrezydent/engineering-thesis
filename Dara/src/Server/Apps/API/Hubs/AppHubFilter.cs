using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Hubs;

public class AppHubFilter : IHubFilter
{
    public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
    {
        try
        {
            return await next(invocationContext);
        }
        catch (BaseDomainException ex)
        {
            Console.WriteLine("DOMAIN EX" + ex.Message + " --- " + ex.GetType().Name);
            throw new HubException(ex.Message);
        }
    }
}