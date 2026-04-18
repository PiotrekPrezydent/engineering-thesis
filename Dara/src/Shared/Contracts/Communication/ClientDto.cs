namespace Dara.Shared.Contracts.Communication;

public record ClientDto(string ClientName, string AuthToken);

public record RegisterClientRequest(ClientDto ClientDto); // check if connection already has a client

public record LeaveClientRequest(); // check if connection even has a client

public record ChangeClientNameRequest(string GivenAuthToken, string NewName); // check if connection even has a client

public record ChangeClientAuthTokenRequest(string GivenAuthToken, string NewToken); // check if connection even has a client

