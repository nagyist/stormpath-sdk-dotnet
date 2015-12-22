﻿// <copyright file="IJwtAuthenticationRequestBuilder.cs" company="Stormpath, Inc.">
// Copyright (c) 2015 Stormpath, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

namespace Stormpath.SDK.Oauth
{
    /// <summary>
    /// Builder pattern used to construct <see cref="IJwtAuthenticationRequest"/> instances.
    /// </summary>
    public interface IJwtAuthenticationRequestBuilder : IOauthAuthenticationRequestBuilder<IJwtAuthenticationRequest>
    {
        /// <summary>
        /// Sets the JWT string to be used for the request.
        /// </summary>
        /// <param name="jwt">The JSON Web Token.</param>
        /// <returns>This instance for method chaining.</returns>
        /// <exception cref="System.ArgumentNullException">JWT is null or empty.</exception>
        IJwtAuthenticationRequestBuilder SetJwt(string jwt);
    }
}
