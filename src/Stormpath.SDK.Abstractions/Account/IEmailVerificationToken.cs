﻿// <copyright file="IEmailVerificationToken.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Account
{
    /// <summary>
    /// Represents a token that can verify the email address for an <see cref="IAccount">Account</see>.
    /// </summary>
    public interface IEmailVerificationToken : IResource, ISaveable<IEmailVerificationToken>
    {
        /// <summary>
        /// Gets a string representation of the email verification token value.
        /// </summary>
        /// <returns>A string representing the token.</returns>
        string GetValue();
    }
}
