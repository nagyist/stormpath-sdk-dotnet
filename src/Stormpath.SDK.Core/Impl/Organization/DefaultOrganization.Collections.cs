﻿// <copyright file="DefaultOrganization.Collections.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Account;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.Linq;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Organization;

namespace Stormpath.SDK.Impl.Organization
{
    internal sealed partial class DefaultOrganization
    {
        IAsyncQueryable<IOrganizationAccountStoreMapping> IAccountStoreContainer<IOrganizationAccountStoreMapping>.GetAccountStoreMappings()
            => new CollectionResourceQueryable<IOrganizationAccountStoreMapping>(this.AccountStoreMappings.Href, this.GetInternalAsyncDataStore());

        IAsyncQueryable<IAccount> IAccountStore.GetAccounts()
            => new CollectionResourceQueryable<IAccount>(this.Accounts.Href, this.GetInternalAsyncDataStore());

        IAsyncQueryable<IGroup> IOrganization.GetGroups()
            => new CollectionResourceQueryable<IGroup>(this.Groups.Href, this.GetInternalAsyncDataStore());
    }
}
