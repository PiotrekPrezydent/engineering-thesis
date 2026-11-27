using Dara.Shared.Contracts;
using Dara.Shared.Contracts.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Dara.Server.Apps.API.Common.Authentication;

// two schemes: the client token (jwt, the default) for clients and the hub, the cookie for the account panel
public static class AuthenticationSetup
{
    public static void AddDaraAuthentication(this WebApplicationBuilder builder)
    {
        var jwt = JwtOptions.FromConfiguration(builder.Configuration);
        builder.Services.AddSingleton(jwt);
        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddSingleton<AccessTokenIssuer>();
        builder.Services.AddSingleton<RevokedClientsRegistry>();

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // claims keep their jwt names (sub, scope, client, sid)
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = jwt.CreateSigningKey(),
                    ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                    ValidateLifetime = true,
                    // one server issues and checks, without it an expired token would work 5 more minutes
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = AuthClaims.Subject
                };
                options.Events = new JwtBearerEvents
                {
                    // a browser cannot set headers on a websocket, so the hub also takes the token from the query
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Query["access_token"];
                        if (!string.IsNullOrEmpty(token) && context.HttpContext.Request.Path.StartsWithSegments("/" + Connections.HubName))
                            context.Token = token;

                        return Task.CompletedTask;
                    },
                    // a revoked client is refused at once, not only when its access token expires; the panel cookie is not a jwt
                    OnTokenValidated = context =>
                    {
                        var registry = context.HttpContext.RequestServices.GetRequiredService<RevokedClientsRegistry>();
                        var client = context.Principal?.FindFirst(AuthClaims.Client)?.Value;
                        if (Guid.TryParse(client, out var clientId) && registry.IsRevoked(clientId))
                            context.Fail("the client was revoked");

                        return Task.CompletedTask;
                    }
                };
            })
            .AddCookie(AccountCookie.Scheme, AccountCookie.Configure);

        builder.Services.AddAuthorization(AuthPolicies.Configure);
    }
}
