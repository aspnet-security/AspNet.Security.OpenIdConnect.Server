﻿/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/AspNet-OpenIdConnect-Server/Owin.Security.OpenIdConnect.Server
 * for more information concerning the license and the contributors participating to this project.
 */

using Microsoft.IdentityModel.Protocols;

namespace AspNet.Security.OpenIdConnect.Server {
    public interface IOpenIdConnectServerFeature {
        OpenIdConnectMessage Request { get; set; }
        OpenIdConnectMessage Response { get; set; }
    }
}
