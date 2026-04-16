namespace Dara.Shared.Contracts;

public record UserCredentials(string  Email, string Password);

public record UserDto(Guid Id, string Email, string Password, string Name);