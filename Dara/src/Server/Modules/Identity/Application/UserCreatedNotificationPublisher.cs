using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Identity.Domain.Events;

namespace Dara.Server.Modules.Identity.Application;

public class UserCreatedNotificationPublisher : IDomainEventNotificationHandler<NewUserCreatedDomainEvent>
{
    public async Task HandleAsync(NewUserCreatedDomainEvent notification)
    {
        Console.WriteLine("Call2 " + notification );
    }
}