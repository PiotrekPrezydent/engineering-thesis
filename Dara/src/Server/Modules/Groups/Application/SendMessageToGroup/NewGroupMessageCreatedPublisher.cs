// using Dara.Server.BuildingBlocks.Application.Events;
// using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
// using Dara.Server.Modules.Groups.Domain.GroupMessages.Events;
// using Dara.Server.Modules.Groups.Integration;
//
// namespace Dara.Server.Modules.Groups.Application.SendMessageToGroup;
//
// public class NewGroupMessageCreatedPublisher : IDomainEventNotificationHandler<NewGroupMessageCreatedDomainEvent>
// {
//     private readonly IEventBus _eventBus;
//
//     public NewGroupMessageCreatedPublisher(IEventBus eventBus)
//     {
//         _eventBus = eventBus;
//     }
//
//     public async Task HandleAsync(NewGroupMessageCreatedDomainEvent notification)
//     {
//         await _eventBus.PublishAsync(new NewGroupMessageCreatedIntegrationEvent(notification.MessageId, notification.GroupId,
//             notification.MemberId, notification.Content));
//     }
// }