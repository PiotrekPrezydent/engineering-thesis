using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Hubs;

public class AppHubFilter : IHubFilter
{
    public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
    {
        // if (invocationContext.Context.Items.TryGetValue("DomainUserId", out var userIdObj) 
        //     && userIdObj is Guid domainUserId)
        // {
        //     // 2. Pobieramy ScopedExecutionContext z DI dla aktualnego wywołania
        //     var executionContext = invocationContext.ServiceProvider
        //         .GetRequiredService<ScopedExecutionContext>();
        //     
        //     // 3. Ustawiamy Guid - od teraz wszystkie moduły używające IExecutionContext 
        //     // w tym wywołaniu będą miały do niego dostęp!
        //     executionContext.UserId = domainUserId;
        // }
        // else
        // {
        //     // Opcjonalnie: Rzuć wyjątek, jeśli ktoś próbuje wysłać komendę bez inicjalizacji sesji
        //     throw new HubException("Nie zainicjowano sesji użytkownika.");
        // }

        // Kontynuuj standardowe wywołanie metody na Hubie
        return await next(invocationContext);
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