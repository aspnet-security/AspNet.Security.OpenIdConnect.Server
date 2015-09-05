﻿/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/aspnet-contrib/AspNet.Security.OpenIdConnect.Server
 * for more information concerning the license and the contributors participating to this project.
 */

using System;
using Microsoft.AspNet.Authentication;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.DataProtection;
using Microsoft.Framework.Caching.Distributed;
using Microsoft.Framework.Internal;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;
using Microsoft.Framework.WebEncoders;

namespace AspNet.Security.OpenIdConnect.Server {
    /// <summary>
    /// Authorization Server middleware component which is added to an OWIN pipeline. This class is not
    /// created by application code directly, instead it is added by calling the the IAppBuilder UseOpenIdConnectServer 
    /// extension method.
    /// </summary>
    public class OpenIdConnectServerMiddleware : AuthenticationMiddleware<OpenIdConnectServerOptions> {
        /// <summary>
        /// Authorization Server middleware component which is added to an OWIN pipeline. This constructor is not
        /// called by application code directly, instead it is added by calling the the IAppBuilder UseOpenIdConnectServer 
        /// extension method.
        /// </summary>
        public OpenIdConnectServerMiddleware(
            [NotNull] RequestDelegate next,
            [NotNull] ILoggerFactory loggerFactory,
            [NotNull] IDistributedCache cache,
            [NotNull] IHtmlEncoder htmlEncoder,
            [NotNull] IUrlEncoder urlEncoder,
            [NotNull] IDataProtectionProvider dataProtectionProvider,
            [NotNull] IOptions<OpenIdConnectServerOptions> options,
            [NotNull] ConfigureOptions<OpenIdConnectServerOptions> configuration)
            : base(next, options, loggerFactory, urlEncoder, configuration) {
            if (Options.AuthorizationCodeFormat == null) {
                Options.AuthorizationCodeFormat = dataProtectionProvider.CreateTicketFormat(
                    typeof(OpenIdConnectServerMiddleware).FullName,
                    Options.AuthenticationScheme, "Authentication_Code", "v1");
            }

            if (Options.AccessTokenFormat == null) {
                Options.AccessTokenFormat = dataProtectionProvider.CreateTicketFormat(
                    typeof(OpenIdConnectServerMiddleware).FullName,
                    Options.AuthenticationScheme, "Access_Token", "v1");
            }

            if (Options.RefreshTokenFormat == null) {
                Options.RefreshTokenFormat = dataProtectionProvider.CreateTicketFormat(
                    typeof(OpenIdConnectServerMiddleware).Namespace,
                    Options.AuthenticationScheme, "Refresh_Token", "v1");
            }

            if (Options.Cache == null) {
                Options.Cache = cache;
            }

            if (Options.HtmlEncoder == null) {
                Options.HtmlEncoder = htmlEncoder;
            }

            if (string.IsNullOrEmpty(Options.AuthenticationScheme)) {
                throw new ArgumentNullException(nameof(Options.AuthenticationScheme));
            }

            if (Options.RandomNumberGenerator == null) {
                throw new ArgumentNullException(nameof(Options.RandomNumberGenerator));
            }

            if (Options.Events == null) {
                throw new ArgumentNullException(nameof(Options.Events));
            }

            if (Options.SystemClock == null) {
                throw new ArgumentNullException(nameof(Options.SystemClock));
            }

            if (Options.Issuer != null) {
                if (!Options.Issuer.IsAbsoluteUri) {
                    throw new ArgumentException("options.Issuer must be a valid absolute URI.", "options.Issuer");
                }

                // See http://openid.net/specs/openid-connect-discovery-1_0.html#IssuerDiscovery
                if (!string.IsNullOrEmpty(Options.Issuer.Query) || !string.IsNullOrEmpty(Options.Issuer.Fragment)) {
                    throw new ArgumentException("options.Issuer must contain no query and no fragment parts.", "options.Issuer");
                }

                // Note: while the issuer parameter should be a HTTPS URI, making HTTPS mandatory
                // in Owin.Security.OpenIdConnect.Server would prevent the end developer from
                // running the different samples in test environments, where HTTPS is often disabled.
                // To mitigate this issue, AllowInsecureHttp can be set to true to bypass the HTTPS check.
                // See http://openid.net/specs/openid-connect-discovery-1_0.html#IssuerDiscovery
                if (!Options.AllowInsecureHttp && string.Equals(Options.Issuer.Scheme, "http", StringComparison.OrdinalIgnoreCase)) {
                    throw new ArgumentException("options.Issuer must be a HTTPS URI when " +
                        "options.AllowInsecureHttp is not set to true.", "options.Issuer");
                }
            }
        }

        /// <summary>
        /// Called by the AuthenticationMiddleware base class to create a per-request handler. 
        /// </summary>
        /// <returns>A new instance of the request handler</returns>
        protected override AuthenticationHandler<OpenIdConnectServerOptions> CreateHandler() {
            return new OpenIdConnectServerHandler();
        }
    }
}
