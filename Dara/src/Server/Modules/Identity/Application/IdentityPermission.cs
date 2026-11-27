using Dara.Server.BuildingBlocks.Application.Authorization;

namespace Dara.Server.Modules.Identity.Application;

// what a user may do in the Identity module; commands and queries do not use it, endpoints of the API are marked with it
public sealed record IdentityPermission(IdentityPermission.Permissions Value) : ModulePermission<IdentityPermission.Permissions>(Value)
{
    public enum Permissions
    {
        ReadClients,
        CreateClientRegistrationCode,
        IssueClientLoginCode,
        RenameClient,
        ChangeClientContext,
        RevokeClient
    }
}
