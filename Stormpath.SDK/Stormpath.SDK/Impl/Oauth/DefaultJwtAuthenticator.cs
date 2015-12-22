﻿// <copyright file="DefaultJwtAuthenticator.cs" company="Stormpath, Inc.">
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Application;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Oauth;

namespace Stormpath.SDK.Impl.Oauth
{
    internal sealed class DefaultJwtAuthenticator :
        IJwtAuthenticator,
        IJwtAuthenticatorSync
    {
        private static readonly string ApplicationPath = "/applications/";
        private static readonly string OauthTokenPath = "/authTokens/";
        private static readonly string AccessTokenPath = "/accessTokens/";

        private readonly IApplication application;
        private readonly IInternalDataStore internalDataStore;

        private bool withLocalValidation = false;

        public DefaultJwtAuthenticator(IApplication application, IInternalDataStore internalDataStore)
        {
            this.application = application;
            this.internalDataStore = internalDataStore;
        }

        private IInternalAsyncDataStore InternalAsyncDataStore => this.internalDataStore as IInternalAsyncDataStore;

        private IInternalSyncDataStore InternalSyncDataStore => this.internalDataStore as IInternalSyncDataStore;

        IJwtAuthenticator IJwtAuthenticator.WithLocalValidation()
        {
            this.withLocalValidation = true;
            return this;
        }

        async Task<IAccessToken> IOauthAuthenticator<IJwtAuthenticationRequest, IAccessToken>
            .AuthenticateAsync(IJwtAuthenticationRequest authenticationRequest, CancellationToken cancellationToken)
        {
            this.ThrowIfInvalid(authenticationRequest);

            return this.withLocalValidation
                ? this.ValidateLocallySync(authenticationRequest)
                : await this.ValidateRemoteAsync(authenticationRequest, cancellationToken).ConfigureAwait(false);
        }

        IAccessToken IOauthAuthenticatorSync<IJwtAuthenticationRequest, IAccessToken>
            .Authenticate(IJwtAuthenticationRequest authenticationRequest)
        {
            this.ThrowIfInvalid(authenticationRequest);

            return this.withLocalValidation
                ? this.ValidateLocallySync(authenticationRequest)
                : this.ValidateRemoteSync(authenticationRequest);
        }

        private Task<IAccessToken> ValidateRemoteAsync(IJwtAuthenticationRequest request, CancellationToken cancellationToken)
        {
            var validationRequestHref = new StringBuilder()
                .Append(this.application.Href)
                .Append(OauthTokenPath)
                .Append(request.Jwt)
                .ToString();

            return this.internalDataStore.GetResourceAsync<IAccessToken>(validationRequestHref);
        }

        private IAccessToken ValidateRemoteSync(IJwtAuthenticationRequest request)
        {
            var validationRequestHref = new StringBuilder()
                .Append(this.application.Href)
                .Append(OauthTokenPath)
                .Append(request.Jwt)
                .ToString();

            return this.InternalSyncDataStore.GetResource<IAccessToken>(validationRequestHref);
        }

        private IAccessToken ValidateLocallySync(IJwtAuthenticationRequest request)
        {
            var apiKeySecret = this.internalDataStore.ApiKey.GetSecret();

            throw new NotImplementedException();
        }

        private void ThrowIfInvalid(IJwtAuthenticationRequest request)
        {
            if (this.application == null)
            {
                throw new ApplicationException($"{nameof(this.application)} cannot be null.");
            }

            if (request == null)
            {
                throw new ApplicationException($"{nameof(request)} cannot be null.");
            }
        }
    }
}
