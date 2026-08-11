using System.Security.Claims;
using System.Text.Encodings.Web;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Application.CreateClientIdentity;
using Dara.Server.Modules.Identity.Application.GetClient;
using Dara.Shared.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Dara.Server.Apps.API.Authentication;

public class ClientIdentifierAuthHandler :  AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IMemoryCache _cache;
    private readonly IIdentityModule _identityModule;
    
    public ClientIdentifierAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, IMemoryCache cache, IIdentityModule identityModule) : base(options, logger, encoder)
    {
        _cache = cache;
        _identityModule = identityModule;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(Connections.IdentifierHeaderName, out var identifier))
        {
            return AuthenticateResult.Fail("No client identifier in headers");
        }
        
      
        List<Claim> claims;
        string cacheKey = $"cache-claims-{identifier.ToString()}";
        
        if (!_cache.TryGetValue(cacheKey, out claims!))
        {
            claims = new List<Claim>();
            
            var clientId = await _identityModule.ExecuteQueryAsync<GetClientQuery, Guid?>(new GetClientQuery(identifier.ToString()));
            
            if(clientId == null)
                clientId = await _identityModule.ExecuteCommandAsync<CreateClientIdentityCommand, Guid>(new CreateClientIdentityCommand(identifier.ToString()));
            
            claims.Add(new Claim(ClaimTypes.NameIdentifier, clientId.ToString()));
            
            _cache.Set(cacheKey, claims);
        }
    
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
    
        return AuthenticateResult.Success(ticket);
    }
}