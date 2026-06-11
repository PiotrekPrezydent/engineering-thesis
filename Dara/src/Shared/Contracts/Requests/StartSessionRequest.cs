using Dara.Shared.Contracts.Dtos;

namespace Dara.Shared.Contracts.Requests;

public record StartSessionRequest(string ClientName, IReadOnlyList<RawPluginDto> Plugins);
