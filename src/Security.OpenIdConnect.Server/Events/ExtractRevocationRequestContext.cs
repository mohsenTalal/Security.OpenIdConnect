/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/aspnet-contrib/Security.OpenIdConnect.Server
 * for more information concerning the license and the contributors participating to this project.
 */

using Security.OpenIdConnect.Primitives;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Security.OpenIdConnect.Server
{
    /// <summary>
    /// Represents the context class associated with the
    /// <see cref="OpenIdConnectServerProvider.ExtractRevocationRequest"/> event.
    /// </summary>
    public class ExtractRevocationRequestContext : BaseValidatingContext
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ExtractRevocationRequestContext"/> class.
        /// </summary>
        public ExtractRevocationRequestContext(
            HttpContext context,
            AuthenticationScheme scheme,
            OpenIdConnectServerOptions options,
            OpenIdConnectRequest request)
            : base(context, scheme, options, request)
        {
            Validate();
        }
    }
}
