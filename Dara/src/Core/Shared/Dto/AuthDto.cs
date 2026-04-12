namespace Dara.Core.Shared.Dto;

public record UserCredentialsDto(string Email, string Password);

public record UserDataDto(string Email, string Password, string Nickname);

public record NodeDto(string Name);
