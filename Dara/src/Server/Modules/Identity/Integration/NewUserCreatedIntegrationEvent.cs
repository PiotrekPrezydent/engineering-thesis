using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Modules.Identity.Integration;

// a new account and its panel client (one per account, created with it); the panel client is the subject of the
// account panel in Authorization, so the roles a module gives a new account go to it (GetNewPanelRoles); it is not a
// client of the hub, no UserClientRegistered follows for it
public record NewUserCreatedIntegrationEvent : IntegrationEventBase
{
    public Guid CreatedUserId { get; }

    public Guid PanelClientId { get; }

    public NewUserCreatedIntegrationEvent(Guid eventId, DateTime occurredOn, Guid createdUserId, Guid panelClientId) : base(eventId, occurredOn)
    {
        CreatedUserId = createdUserId;
        PanelClientId = panelClientId;
    }
}
