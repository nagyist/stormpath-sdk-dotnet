﻿// <copyright file="IAsynchronousFilterChain.cs" company="Stormpath, Inc.">
// Copyright (c) 2016 Stormpath, Inc.
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

using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Logging;

namespace Stormpath.SDK.Impl.DataStore.Filters
{
    /// <summary>
    /// Represents an ordered chain of filters to execute for an asynchronous request.
    /// </summary>
    internal interface IAsynchronousFilterChain
    {
        /// <summary>
        /// Gets the data store invoking this filter chain.
        /// </summary>
        /// <value>The data store.</value>
        IInternalAsyncDataStore DataStore { get; }

        /// <summary>
        /// Executes each filter in the chain for an asynchronous request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The result of the filtered request.</returns>
        Task<IResourceDataResult> FilterAsync(IResourceDataRequest request, ILogger logger, CancellationToken cancellationToken);
    }
}
