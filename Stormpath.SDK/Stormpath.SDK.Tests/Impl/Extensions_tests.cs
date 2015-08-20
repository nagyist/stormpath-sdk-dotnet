﻿// <copyright file="Extensions_tests.cs" company="Stormpath, Inc.">
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

using Shouldly;
using Stormpath.SDK.Impl.Extensions;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class Extensions_tests
    {
        public class String_Nullable
        {
            [Fact]
            public void Returns_null_when_string_is_null()
            {
                ((string)null).Nullable().ShouldBe(null);
            }

            [Fact]
            public void Returns_null_when_string_is_empty()
            {
                string.Empty.Nullable().ShouldBe(null);
            }

            [Fact]
            public void Returns_string()
            {
                "foobar".Nullable().ShouldBe("foobar");
            }
        }

        public class String_ToBase64
        {
            [Fact]
            public void Returns_null_when_string_is_null()
            {
                ((string)null).ToBase64(System.Text.Encoding.UTF8).ShouldBe(null);
            }

            [Fact]
            public void Returns_empty_when_string_is_empty()
            {
                string.Empty.ToBase64(System.Text.Encoding.UTF8).ShouldBe(string.Empty);
            }

            [Fact]
            public void Returns_Zm9vYmFy_for_foobar()
            {
                "foobar".ToBase64(System.Text.Encoding.UTF8).ShouldBe("Zm9vYmFy");
            }
        }

        public class String_FromBase64
        {
            [Fact]
            public void Returns_null_when_string_is_null()
            {
                ((string)null).FromBase64(System.Text.Encoding.UTF8).ShouldBe(null);
            }

            [Fact]
            public void Returns_empty_when_string_is_empty()
            {
                string.Empty.FromBase64(System.Text.Encoding.UTF8).ShouldBe(string.Empty);
            }

            [Fact]
            public void Returns_Zm9vYmFy_for_foobar()
            {
                "Zm9vYmFy".FromBase64(System.Text.Encoding.UTF8).ShouldBe("foobar");
            }
        }
    }
}