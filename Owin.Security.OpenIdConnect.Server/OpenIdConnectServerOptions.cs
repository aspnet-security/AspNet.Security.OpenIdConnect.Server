﻿namespace Microsoft.Owin.Security.OpenIdConnect.Server {
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens;
    using System.Security.Claims;
    using Microsoft.Owin.Infrastructure;
    using Microsoft.Owin.Security.Infrastructure;

    /// <summary>
    /// Options class provides information needed to control Authorization Server middleware behavior
    /// </summary>
    public class OpenIdConnectServerOptions : AuthenticationOptions {
        /// <summary>
        /// Creates an instance of authorization server options with default values.
        /// </summary>
        public OpenIdConnectServerOptions()
            : base(OpenIdConnectDefaults.AuthenticationType) {
            AuthorizationCodeExpireTimeSpan = TimeSpan.FromMinutes(5);
            AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(20);
            SystemClock = new SystemClock();
            TokenHandler = new JwtSecurityTokenHandler();
            ServerClaimsMapper = claims => claims;
            IdTokenExpireTimeSpan = TimeSpan.FromMinutes(20);
            FormPostEndpointPath = new PathString("/openid/form_post");
            UseDefaultForm = true;
        }

        /// <summary>
        /// The request path where client applications will redirect the user-agent in order to 
        /// obtain user consent to issue a token. Must begin with a leading slash, like "/Authorize".
        /// </summary>
        public PathString AuthorizeEndpointPath { get; set; }

        /// <summary>
        /// The intermediate request path where the user-agent is redirect to before being redirect to
        /// the client application when using response_mode=form. Must begin with a leading slash, like "/FormPost".
        /// </summary>
        public PathString FormPostEndpointPath { get; set; }

        /// <summary>
        /// The request path client applications communicate with directly as part of the OpenID Connect protocol. 
        /// Must begin with a leading slash, like "/Token". If the client is issued a client_secret, it must
        /// be provided to this endpoint.
        /// </summary>
        public PathString TokenEndpointPath { get; set; }

        /// <summary>
        /// The object provided by the application to process events raised by the Authorization Server middleware.
        /// The application may implement the interface fully, or it may create an instance of OpenIdConnectServerProvider
        /// and assign delegates only to the events it wants to process.
        /// </summary>
        public IOpenIdConnectServerProvider Provider { get; set; }

        /// <summary>
        /// The data format used to protect and unprotect the information contained in the authorization code. 
        /// If not provided by the application the default data protection provider depends on the host server. 
        /// The SystemWeb host on IIS will use ASP.NET machine key data protection, and HttpListener and other self-hosted
        /// servers will use DPAPI data protection.
        /// </summary>
        public ISecureDataFormat<AuthenticationTicket> AuthorizationCodeFormat { get; set; }

        /// <summary>
        /// The data format used to protect the information contained in the access token. 
        /// If not provided by the application the default data protection provider depends on the host server. 
        /// The SystemWeb host on IIS will use ASP.NET machine key data protection, and HttpListener and other self-hosted
        /// servers will use DPAPI data protection. If a different access token
        /// provider or format is assigned, a compatible instance must be assigned to the OAuthBearerAuthenticationOptions.AccessTokenProvider 
        /// or OAuthBearerAuthenticationOptions.AccessTokenFormat property of the resource server.
        /// </summary>
        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; set; }

        /// <summary>
        /// The data format used to protect and unprotect the information contained in the refresh token. 
        /// If not provided by the application the default data protection provider depends on the host server. 
        /// The SystemWeb host on IIS will use ASP.NET machine key data protection, and HttpListener and other self-hosted
        /// servers will use DPAPI data protection.
        /// </summary>
        public ISecureDataFormat<AuthenticationTicket> RefreshTokenFormat { get; set; }

        /// <summary>
        /// The period of time the authorization code remains valid after being issued. The default is five minutes.
        /// This time span must also take into account clock synchronization between servers in a web farm, so a very 
        /// brief value could result in unexpectedly expired tokens.
        /// </summary>
        public TimeSpan AuthorizationCodeExpireTimeSpan { get; set; }

        /// <summary>
        /// The period of time the access token remains valid after being issued. The default is twenty minutes.
        /// The client application is expected to refresh or acquire a new access token after the token has expired. 
        /// </summary>
        public TimeSpan AccessTokenExpireTimeSpan { get; set; }

        /// <summary>
        /// Produces a single-use authorization code to return to the client application. For the OpenID Connect server to be secure the
        /// application MUST provide an instance for AuthorizationCodeProvider where the token produced by the OnCreate or OnCreateAsync event 
        /// is considered valid for only one call to OnReceive or OnReceiveAsync. 
        /// </summary>
        public IAuthenticationTokenProvider AuthorizationCodeProvider { get; set; }

        /// <summary>
        /// Produces a bearer token the client application will typically be providing to resource server as the authorization bearer 
        /// http request header. If not provided the token produced on the server's default data protection. If a different access token
        /// provider or format is assigned, a compatible instance must be assigned to the OAuthBearerAuthenticationOptions.AccessTokenProvider 
        /// or OAuthBearerAuthenticationOptions.AccessTokenFormat property of the resource server.
        /// </summary>
        public IAuthenticationTokenProvider AccessTokenProvider { get; set; }

        /// <summary>
        /// Produces a refresh token which may be used to produce a new access token when needed. If not provided the authorization server will
        /// not return refresh tokens from the /Token endpoint.
        /// </summary>
        public IAuthenticationTokenProvider RefreshTokenProvider { get; set; }

        /// <summary>
        /// Set to true if the web application is able to render error messages on the /Authorize endpoint. This is only needed for cases where
        /// the browser is not redirected back to the client application, for example, when the client_id or redirect_uri are incorrect. The 
        /// /Authorize endpoint should expect to see "oauth.Error", "oauth.ErrorDescription", "oauth.ErrorUri" properties added to the owin environment.
        /// </summary>
        public bool ApplicationCanDisplayErrors { get; set; }

        /// <summary>
        /// Used to know what the current clock time is when calculating or validating token expiration. When not assigned default is based on
        /// DateTimeOffset.UtcNow. This is typically needed only for unit testing.
        /// </summary>
        public ISystemClock SystemClock { get; set; }

        /// <summary>
        /// True to allow authorize and token requests to arrive on http URI addresses, and to allow incoming 
        /// redirect_uri authorize request parameter to have http URI addresses.
        /// </summary>
        public bool AllowInsecureHttp { get; set; }

        public TimeSpan IdTokenExpireTimeSpan { get; set; }
        public string IssuerName { get; set; }
        public Func<IEnumerable<Claim>, IEnumerable<Claim>> ServerClaimsMapper { get; set; }
        public SignatureProvider SignatureProvider { get; set; }
        public SigningCredentials SigningCredentials { get; set; }
        public JwtSecurityTokenHandler TokenHandler { get; set; }
        public ISecureDataFormat<OpenIdConnectPayload> PayloadFormat { get; set; }
        public bool UseDefaultForm { get; set; }
    }
}
