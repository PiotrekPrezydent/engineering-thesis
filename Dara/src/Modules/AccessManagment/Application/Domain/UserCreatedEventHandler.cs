using Dara.BuildingBlocks.Domain.Events;
using Dara.Modules.AccessManagment.Domain.Users.Events;

namespace Dara.Modules.AccessManagment.Application.Domain;

public class UserCreatedEventHandler : IDomainEventHandler<UserCreatedEvent>
{
    public async Task HandleAsync(UserCreatedEvent domainEvent)
    {
        Console.WriteLine("User created");
    }
}