﻿// <copyright file="ApiKey_tests.cs" company="Stormpath, Inc.">
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

using System.Linq;
using Shouldly;
using Stormpath.SDK.Api;
using Stormpath.SDK.Sync;
using Stormpath.SDK.Tests.Common.Integration;
using Stormpath.SDK.Tests.Common.RandomData;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Sync
{
    [Collection(nameof(IntegrationTestCollection))]
    public class ApiKey_tests
    {
        private readonly TestFixture fixture;

        public ApiKey_tests(TestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Creating_and_deleting_api_key(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = client.GetApplication(this.fixture.PrimaryApplicationHref);
            var account = app.CreateAccount("ApiKey", "Tester1", "api-key-tester-1@foo.foo", new RandomPassword(12));
            this.fixture.CreatedAccountHrefs.Add(account.Href);

            account.GetApiKeys().Synchronously().Count().ShouldBe(0);

            var newKey1 = account.CreateApiKey();
            var newKey2 = account.CreateApiKey(opt =>
            {
                opt.Expand(e => e.GetAccount());
                opt.Expand(e => e.GetTenant());
            });

            var keysList = account.GetApiKeys().Synchronously().ToList();
            keysList.Count.ShouldBe(2);
            keysList.ShouldContain(x => x.Href == newKey1.Href);
            keysList.ShouldContain(x => x.Href == newKey2.Href);

            newKey1.Delete().ShouldBeTrue();
            newKey2.Delete().ShouldBeTrue();

            // Clean up
            account.Delete().ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Updating_api_key(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = client.GetApplication(this.fixture.PrimaryApplicationHref);
            var account = app.CreateAccount("ApiKey", "Tester2", "api-key-tester-2@foo.foo", new RandomPassword(12));
            this.fixture.CreatedAccountHrefs.Add(account.Href);

            var newKey = account.CreateApiKey();
            newKey.Status.ShouldBe(ApiKeyStatus.Enabled);

            newKey.SetStatus(ApiKeyStatus.Disabled);
            newKey.Save();
            newKey.Status.ShouldBe(ApiKeyStatus.Disabled);

            var retrieved = account.GetApiKeys().Synchronously().Single();
            retrieved.Status.ShouldBe(ApiKeyStatus.Disabled);

            // Clean up
            newKey.Delete().ShouldBeTrue();

            account.Delete().ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Looking_up_api_key_via_application(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = client.GetApplication(this.fixture.PrimaryApplicationHref);
            var account = app.CreateAccount("ApiKey", "Tester3", "api-key-tester-3@foo.foo", new RandomPassword(12));
            this.fixture.CreatedAccountHrefs.Add(account.Href);

            var newKey = account.CreateApiKey();

            var foundKey = app.GetApiKey(newKey.Id, opt =>
            {
                opt.Expand(e => e.GetAccount());
                opt.Expand(e => e.GetTenant());
            });
            var foundAccount = foundKey.GetAccount();
            foundAccount.Href.ShouldBe(account.Href);

            // Clean up
            newKey.Delete().ShouldBeTrue();

            account.Delete().ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }
    }
}
