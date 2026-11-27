using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.Modules.Identity.Domain.Clients.Events;
using Dara.Server.Modules.Identity.Domain.Clients.Rules;
using Dara.Server.Modules.Identity.Domain.Users;

namespace Dara.Server.Modules.Identity.Domain.Clients;

// a subject that acts for an account and holds its own roles in Authorization: an app of the hub (the CLI on a
// computer, the phone app) or the account panel over http (one per account); it belongs to one account for good, so
// a change of one client never loads the account or its other clients; a revoked client takes no change at all
public class Client : Entity, IAggregateRoot
{
    public ClientId Id { get; private set; }
    public UserId UserId { get; private set; }
    public ClientKind Kind { get; private set; }
    public DateTime RegisteredAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    // null for the panel client
    public HubProfile? Hub { get; private set; }

    public bool IsRevoked => RevokedAt != null;

    private Client() { }

    private Client(UserId userId, ClientKind kind, HubProfile? hub, DateTime now)
    {
        Id = new ClientId(Guid.NewGuid());
        UserId = userId;
        Kind = kind;
        Hub = hub;
        RegisteredAt = now;
    }

    // the application has used up the registration code of the account in the same unit of work; an empty name gets
    // the label of the type with a number from the count of the clients of this type in the account (revoked ones
    // too, so a default name is not reused; two parallel registrations may get the same number, names need not be
    // unique)
    public static Client RegisterHub(UserId userId, ClientType type, string? name, int sameTypeCount, DateTime now)
    {
        var clientName = string.IsNullOrWhiteSpace(name) ? ClientTypeLabels.DefaultName(type, sameTypeCount) : name.Trim();
        CheckStaticRule(new ClientNameMustBeValidRule(clientName));

        var client = new Client(userId, ClientKind.Hub, HubProfile.Create(clientName, type), now);
        client.AddDomainEvent(new UserClientRegisteredDomainEvent(userId, client.Id, clientName, type));
        return client;
    }

    // created with the account, in the same unit of work
    public static Client CreatePanel(UserId userId, DateTime now)
    {
        var client = new Client(userId, ClientKind.Panel, null, now);
        client.AddDomainEvent(new PanelClientCreatedDomainEvent(userId, client.Id));
        return client;
    }

    // a client of another account looks to the caller like an unknown one or breaks this rule
    public void EnsureOwnedBy(UserId userId)
    {
        CheckRule(new ClientMustBelongToUserRule(UserId == userId));
    }

    // for an existing client of the hub only (a copy of the app on another computer); a new code replaces the
    // previous one
    public ClientLoginCode IssueLoginCode(string codeHash, DateTime now, TimeSpan lifetime)
    {
        var hub = GetHubForChange();
        return hub.IssueLoginCode(codeHash, now, lifetime);
    }

    public bool HasLoginCode(string codeHash)
    {
        return Hub is not null && Hub.HasLoginCode(codeHash);
    }

    // uses up the code; false when it is not the open code or it expired; the application then signs in the session
    // of this client in the same unit of work
    public bool ConsumeLoginCode(string codeHash, DateTime now)
    {
        var hub = GetHubForChange();
        return hub.ConsumeLoginCode(codeHash, now);
    }

    public void Rename(string name)
    {
        var hub = GetHubForChange();
        if (!hub.Rename(name))
            return;

        AddDomainEvent(new UserClientRenamedDomainEvent(UserId, Id, hub.Name));
    }

    // replaces the whole context of the client
    public void ChangeContext(IReadOnlyDictionary<string, string> entries)
    {
        var hub = GetHubForChange();
        if (!hub.ChangeContext(entries))
            return;

        AddDomainEvent(new UserClientContextChangedDomainEvent(UserId, Id, hub.Context));
    }

    // the application ends the session of the client in the same unit of work
    public void Revoke(DateTime now)
    {
        var hub = GetHubForChange();
        RevokedAt = now;
        hub.DropLoginCode();

        AddDomainEvent(new UserClientRevokedDomainEvent(UserId, Id));
    }

    private HubProfile GetHubForChange()
    {
        CheckRule(new ClientMustBeHubClientRule(Kind));
        CheckRule(new ClientMustNotBeRevokedRule(IsRevoked));
        return Hub!;
    }

    // the client does not exist yet when its name is checked, so the instance method of Entity does not fit
    private static void CheckStaticRule(IBuisnessRule rule)
    {
        if (rule.IsBroken())
            throw new BuisnessRuleValidationException(rule);
    }
}
