namespace Dara.Shared.Contracts.Clients;

public record ClientActivationDto(string ClientName, string ClientAuthToken);

public record ClientNameDto(string ClientName);

public record ClientAuthTokenDto(string ClientAuthToken);