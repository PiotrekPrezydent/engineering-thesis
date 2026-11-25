using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Hubs;

public class AppHubFilter : IHubFilter
{
    private readonly ILogger<AppHubFilter> _logger;

    public AppHubFilter(ILogger<AppHubFilter> logger)
    {
        _logger = logger;
    }

    public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
    {
        try
        {
            return await next(invocationContext);
        }
        catch (BuisnessRuleValidationException ex)
        {
            // a broken rule is an answer for the caller, its message says what to change
            _logger.LogInformation("{Method} broke {Rule}: {Message}", invocationContext.HubMethodName, ex.BrokenRule.GetType().Name, ex.Message);
            throw new HubException(ex.Message);
        }
        catch (BaseDomainException ex)
        {
            _logger.LogInformation("{Method} failed in the domain: {Message}", invocationContext.HubMethodName, ex.Message);
            throw new HubException(ex.Message);
        }
        catch (HubException)
        {
            throw;
        }
        catch (Exception e)
        {
            // details stay in the server log, the caller gets no internals
            _logger.LogError(e, "{Method} failed", invocationContext.HubMethodName);
            throw new HubException($"{invocationContext.HubMethodName} failed on the server");
        }
    }
}