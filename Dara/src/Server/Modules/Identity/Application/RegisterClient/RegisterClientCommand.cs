using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Identity.Application.RegisterClient;

// the app of a new client registers itself with the code of the panel, never with the password; ClientType is text
// without regard to case (unknown text is Other), an empty name gets the label of the type with a number
public record RegisterClientCommand(string Code, string? ClientType, string? Name) : ICommand<ClientTokensResult>;
