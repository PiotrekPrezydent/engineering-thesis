using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Identity.Domain.Events;

namespace Dara.Server.Modules.Identity.Application;

public class UserCreatedNotificationHandler : IDomainEventNotificationHandler<NewUserCreatedDomainEvent>
{
    public async Task HandleAsync(NewUserCreatedDomainEvent notification)
    {
        Console.WriteLine("Call1 " + notification );
    }
}