Security.OpenIdConnect.Server
==================================

**Security.OpenIdConnect.Server** is an **advanced OAuth2/OpenID Connect server framework** for both ASP.NET Core 1.x/2.x and OWIN/Katana 3.x/4.x, designed to offer a low-level, protocol-first approach.


[![Build status](https://ci.appveyor.com/api/projects/status/tyenw4ffs00j4sav/branch/dev?svg=true)](https://ci.appveyor.com/project/mohsenTalal/security-openidconnect/branch/master)
[![Build status](https://api.travis-ci.org/mohsenTalal/Security.OpenIdConnect.svg?branch=master)](https://travis-ci.org/mohsenTalal/Security.OpenIdConnect)

## Get started

Based on `OAuthAuthorizationServerMiddleware` from **Mohsen**, **Security.OpenIdConnect.Server** exposes similar primitives and can be directly registered in **Startup.cs** using the `UseOpenIdConnectServer` extension method:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddAuthentication().AddOpenIdConnectServer(options =>
    {
        // Enable the token endpoint.
        options.TokenEndpointPath = "/connect/token";
    
        // Implement OnValidateTokenRequest to support flows using the token endpoint.
        options.Provider.OnValidateTokenRequest = context =>
        {
            // Reject token requests that don't use grant_type=password or grant_type=refresh_token.
            if (!context.Request.IsPasswordGrantType() && !context.Request.IsRefreshTokenGrantType())
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.UnsupportedGrantType,
                    description: "Only grant_type=password and refresh_token " +
                                 "requests are accepted by this server.");
    
                return Task.CompletedTask;
            }
    
            // Note: you can skip the request validation when the client_id
            // parameter is missing to support unauthenticated token requests.
            // if (string.IsNullOrEmpty(context.ClientId))
            // {
            //     context.Skip();
            // 
            //     return Task.CompletedTask;
            // }
    
            // Note: to mitigate brute force attacks, you SHOULD strongly consider applying
            // a key derivation function like PBKDF2 to slow down the secret validation process.
            // You SHOULD also consider using a time-constant comparer to prevent timing attacks.
            if (string.Equals(context.ClientId, "client_id", StringComparison.Ordinal) &&
                string.Equals(context.ClientSecret, "client_secret", StringComparison.Ordinal))
            {
                context.Validate();
            }
    
            // Note: if Validate() is not explicitly called,
            // the request is automatically rejected.
            return Task.CompletedTask;
        };
    
        // Implement OnHandleTokenRequest to support token requests.
        options.Provider.OnHandleTokenRequest = context =>
        {
            // Only handle grant_type=password token requests and let
            // the OpenID Connect server handle the other grant types.
            if (context.Request.IsPasswordGrantType())
            {
                // Implement context.Request.Username/context.Request.Password validation here.
                // Note: you can call context Reject() to indicate that authentication failed.
                // Using password derivation and time-constant comparer is STRONGLY recommended.
                if (!string.Equals(context.Request.Username, "Bob", StringComparison.Ordinal) ||
                    !string.Equals(context.Request.Password, "P@ssw0rd", StringComparison.Ordinal))
                {
                    context.Reject(
                        error: OpenIdConnectConstants.Errors.InvalidGrant,
                        description: "Invalid user credentials.");
    
                    return Task.CompletedTask;
                }
    
                var identity = new ClaimsIdentity(context.Scheme.Name,
                    OpenIdConnectConstants.Claims.Name,
                    OpenIdConnectConstants.Claims.Role);
    
                // Add the mandatory subject/user identifier claim.
                identity.AddClaim(OpenIdConnectConstants.Claims.Subject, "[unique id]");
    
                // By default, claims are not serialized in the access/identity tokens.
                // Use the overload taking a "destinations" parameter to make sure
                // your claims are correctly inserted in the appropriate tokens.
                identity.AddClaim("urn:customclaim", "value",
                    OpenIdConnectConstants.Destinations.AccessToken,
                    OpenIdConnectConstants.Destinations.IdentityToken);
    
                var ticket = new AuthenticationTicket(
                    new ClaimsPrincipal(identity),
                    new AuthenticationProperties(),
                    context.Scheme.Name);
    
                // Call SetScopes with the list of scopes you want to grant
                // (specify offline_access to issue a refresh token).
                ticket.SetScopes(
                    OpenIdConnectConstants.Scopes.Profile,
                    OpenIdConnectConstants.Scopes.OfflineAccess);
    
                context.Validate(ticket);
            }
    
            return Task.CompletedTask;
        };
    });
}
```

> Note: in order for the OpenID Connect server to work properly, **the authentication middleware must be registered in the ASP.NET Core 2.0 pipeline**:

```csharp
public void Configure(IApplicationBuilder app)
{
    app.UseAuthentication();
}
```

> Note: **the Security.OpenIdConnect.Server 2.x packages are only compatible with ASP.NET Core 2.x**.
> If your application targets ASP.NET Core 1.x, use the Security.OpenIdConnect.Server 1.x packages.

## Support

**Need help or wanna share your thoughts?** Don't hesitate to join us on Gitter or ask your question on StackOverflow:

- **StackOverflow: [https://stackexchange.com/users/13936221/abdul-mohsen-al-enazi](https://stackexchange.com/users/13936221/abdul-mohsen-al-enazi)**

## Contributors

**Security.OpenIdConnect.Server** is actively maintained by **[Mohsen Talal](https://github.com/mohsenTalal)**. Contributions are welcome and can be submitted using pull requests.

