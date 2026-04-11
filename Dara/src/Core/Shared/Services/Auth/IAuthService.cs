namespace Dara.Core.Shared.Services.Auth;

public record LoginRequest(string Email, string Password) : IDaraServiceRequest;
public record LoginResponse(Guid UserId) : IDaraServiceResponse;

public record RegisterRequest(string Email, string Password) : IDaraServiceRequest;
public record RegisterResponse() : IDaraServiceResponse;

public record LogoutRequest(Guid UserId) : IDaraServiceRequest;
public record LoqoutResponse() : IDaraServiceResponse;

public interface IAuthService : IDaraService
{
    public Task<LoginResponse> LoginAsync(LoginRequest request);
    
    public Task<RegisterResponse> RegisterAsync(RegisterRequest request);
    
    public Task<LogoutRequest> LogoutAsync(LogoutRequest request);
}