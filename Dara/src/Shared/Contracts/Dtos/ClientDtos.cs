namespace Dara.Shared.Contracts.Dtos
{
    public record ClientActivationDto(string ClientName, string ClientAuthToken) : IDto;

    public record ClientNameDto(string ClientName) : IDto;

    public record ClientAuthTokenDto(string ClientAuthToken) : IDto;
}