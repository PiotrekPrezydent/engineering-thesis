namespace Dara.Server.Modules.Identity.Application;

public record IdentityOptions
{
    // a client session: every new refresh token gets the full lifetime (30 days from the last refresh)
    public TimeSpan RefreshTokenLifetime { get; init; } = TimeSpan.FromDays(30);

    // a sign-in of the panel (the cookie of a browser) ends after this time without a request
    public TimeSpan PanelSignInIdleLifetime { get; init; } = TimeSpan.FromDays(30);

    // a registration code of the panel registers one new client within this time
    public TimeSpan ClientRegistrationCodeLifetime { get; init; } = TimeSpan.FromMinutes(10);

    // a login code of the panel is used once within this time
    public TimeSpan ClientLoginCodeLifetime { get; init; } = TimeSpan.FromMinutes(10);
}
