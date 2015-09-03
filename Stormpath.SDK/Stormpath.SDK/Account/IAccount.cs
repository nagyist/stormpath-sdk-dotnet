﻿// <copyright file="IAccount.cs" company="Stormpath, Inc.">
//      Copyright (c) 2015 Stormpath, Inc.
// </copyright>
// <remarks>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </remarks>

using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Account
{
    public interface IAccount : IResource, ISaveable<IAccount>, IDeletable, IExtendable, IAuditable
    {
        string Username { get; }

        string Email { get; }

        string FullName { get; }

        string GivenName { get; }

        string MiddleName { get; }

        string Surname { get; }

        AccountStatus Status { get; }

        IAccount SetEmail(string email);

        IAccount SetGivenName(string givenName);

        IAccount SetMiddleName(string middleName);

        IAccount SetPassword(string password);

        IAccount SetStatus(AccountStatus status);

        IAccount SetSurname(string surname);

        IAccount SetUsername(string username);

        Task<ICollectionResourceQueryable<IGroup>> GetGroupsAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<IDirectory> GetDirectoryAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<ITenant> GetTenantAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
