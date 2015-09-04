﻿// <copyright file="DefaultCache_tests.cs" company="Stormpath, Inc.">
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

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Impl.Cache;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Cache
{
    public class DefaultCache_tests
    {
        private static readonly TimeSpan OneMinuteExpiration = TimeSpan.FromMinutes(1);

        public class SynchronousCache
        {
            [Fact]
            public void Empty_cache_is_empty()
            {
                var cache = new DefaultCache<string, string>("fooCache");

                (cache as ISynchronousCache<string, string>).Name.ShouldBe("fooCache");
                cache.TimeToLive.ShouldBe(null);
                cache.TimeToIdle.ShouldBe(null);
                cache.Size.ShouldBe(0);
                cache.AccessCount.ShouldBe(0);
                cache.HitCount.ShouldBe(0);
                cache.MissCount.ShouldBe(0);
                cache.GetHitRatio().ShouldBe(0);
            }

            [Fact]
            public void Cache_access_hit()
            {
                var cache = new DefaultCache<string, string>(
                    "fooCache",
                    timeToLive: OneMinuteExpiration,
                    timeToIdle: OneMinuteExpiration);
                var iface = cache as ISynchronousCache<string, string>;

                iface.Put("foo", "bar");
                cache.Size.ShouldBe(1);

                iface.Get("foo").ShouldBe("bar");
                cache.AccessCount.ShouldBe(1);
                cache.HitCount.ShouldBe(1);
                cache.MissCount.ShouldBe(0);
                cache.GetHitRatio().ShouldBe(1.0);
            }

            [Fact]
            public void Cache_access_miss()
            {
                var cache = new DefaultCache<string, string>(
                    "fooCache",
                    timeToLive: OneMinuteExpiration,
                    timeToIdle: OneMinuteExpiration);
                var iface = cache as ISynchronousCache<string, string>;

                iface.Put("foo", "bar");

                iface.Get("baz").ShouldBe(null);
                cache.AccessCount.ShouldBe(1);
                cache.HitCount.ShouldBe(0);
                cache.MissCount.ShouldBe(1);
                cache.GetHitRatio().ShouldBe(0.0);
            }

            [Fact]
            public void Removing_item()
            {
                var cache = new DefaultCache<string, string>(
                    "fooCache",
                    timeToLive: OneMinuteExpiration,
                    timeToIdle: OneMinuteExpiration);
                var iface = cache as ISynchronousCache<string, string>;

                iface.Put("foo", "bar");
                iface.Remove("foo");

                iface.Get("bar").ShouldBe(null);
                cache.AccessCount.ShouldBe(2);
            }

            [Fact]
            public void Multiple_cache_reads_and_writes_from_single_thread()
            {
                var cache = new DefaultCache<string, string>(
                    "fooCache",
                    timeToLive: OneMinuteExpiration,
                    timeToIdle: OneMinuteExpiration);
                var iface = cache as ISynchronousCache<string, string>;
                iface.Put("foo", "bar");

                for (var i = 0; i < 10; i++)
                {
                    iface.Put(i.ToString(), $"loop{i}");
                    iface.Get(i.ToString()).ShouldBe($"loop{i}");
                    iface.Get("foo").ShouldBe("bar");
                    iface.Get("baz").ShouldBe(null);
                }

                cache.Size.ShouldBe(11);
                cache.AccessCount.ShouldBe(30);
                cache.HitCount.ShouldBe(20);
                cache.MissCount.ShouldBe(10);
                cache.GetHitRatio().ShouldBe(0.67, tolerance: 0.01);
            }

            [Fact]
            public void Accessing_cache_from_multiple_threads()
            {
                var cache = new DefaultCache<string, string>(
                    "fooCache",
                    timeToLive: OneMinuteExpiration,
                    timeToIdle: OneMinuteExpiration);
                var iface = cache as ISynchronousCache<string, string>;
                iface.Put("foo", "bar");

                Parallel.For(0, 10, i =>
                {
                    iface.Put(i.ToString(), $"loop{i}");
                    iface.Get(i.ToString()).ShouldBe($"loop{i}");
                    iface.Get("foo").ShouldBe("bar");
                    iface.Get("baz").ShouldBe(null);
                });

                cache.Size.ShouldBe(11);
                cache.AccessCount.ShouldBe(30);
                cache.HitCount.ShouldBe(20);
                cache.MissCount.ShouldBe(10);
                cache.GetHitRatio().ShouldBe(0.67, tolerance: 0.01);
            }

            [Fact]
            public void Cache_access_after_TTL_expiration()
            {
                var cache = new DefaultCache<string, string>(
                    "fooCache",
                    timeToLive: TimeSpan.FromMilliseconds(50),
                    timeToIdle: OneMinuteExpiration);
                var iface = cache as ISynchronousCache<string, string>;

                iface.Put("foo", "bar");

                Thread.Sleep(10);
                iface.Get("foo").ShouldBe("bar");

                Thread.Sleep(50);
                iface.Get("foo").ShouldBe(null);
            }

            [Fact]
            public void Cache_access_after_TTI_expiration()
            {
                var cache = new DefaultCache<string, string>(
                    "fooCache",
                    timeToLive: OneMinuteExpiration,
                    timeToIdle: TimeSpan.FromMilliseconds(100));
                var iface = cache as ISynchronousCache<string, string>;

                iface.Put("foo", "bar");

                Thread.Sleep(50);
                iface.Get("foo").ShouldBe("bar");

                Thread.Sleep(50);
                iface.Get("foo").ShouldBe("bar");

                Thread.Sleep(125);
                iface.Get("foo").ShouldBe(null);
            }
        }

        public class AsynchronousCache
        {
            [Fact]
            public void Empty_cache_is_empty()
            {
                var cache = new DefaultCache<string, string>("fooCache");

                (cache as IAsynchronousCache<string, string>).Name.ShouldBe("fooCache");
                cache.TimeToLive.ShouldBe(null);
                cache.TimeToIdle.ShouldBe(null);
                cache.Size.ShouldBe(0);
                cache.AccessCount.ShouldBe(0);
                cache.HitCount.ShouldBe(0);
                cache.MissCount.ShouldBe(0);
                cache.GetHitRatio().ShouldBe(0);
            }

            [Fact]
            public async Task Cache_access_hit()
            {
                var cache = new DefaultCache<string, string>(
                    "fooCache",
                    timeToLive: OneMinuteExpiration,
                    timeToIdle: OneMinuteExpiration);
                var iface = cache as IAsynchronousCache<string, string>;

                await iface.PutAsync("foo", "bar");
                cache.Size.ShouldBe(1);

                (await iface.GetAsync("foo")).ShouldBe("bar");
                cache.AccessCount.ShouldBe(1);
                cache.HitCount.ShouldBe(1);
                cache.MissCount.ShouldBe(0);
                cache.GetHitRatio().ShouldBe(1.0);
            }

            [Fact]
            public async Task Cache_access_miss()
            {
                var cache = new DefaultCache<string, string>(
                    "fooCache",
                    timeToLive: OneMinuteExpiration,
                    timeToIdle: OneMinuteExpiration);
                var iface = cache as IAsynchronousCache<string, string>;

                await iface.PutAsync("foo", "bar");

                (await iface.GetAsync("baz")).ShouldBe(null);
                cache.AccessCount.ShouldBe(1);
                cache.HitCount.ShouldBe(0);
                cache.MissCount.ShouldBe(1);
                cache.GetHitRatio().ShouldBe(0.0);
            }

            [Fact]
            public async Task Removing_item()
            {
                var cache = new DefaultCache<string, string>(
                    "fooCache",
                    timeToLive: OneMinuteExpiration,
                    timeToIdle: OneMinuteExpiration);
                var iface = cache as IAsynchronousCache<string, string>;

                await iface.PutAsync("foo", "bar");
                await iface.RemoveAsync("foo");

                (await iface.GetAsync("bar")).ShouldBe(null);
                cache.AccessCount.ShouldBe(2);
            }

            [Fact]
            public async Task Multiple_cache_reads_and_writes_from_single_thread()
            {
                var cache = new DefaultCache<string, string>(
                    "fooCache",
                    timeToLive: OneMinuteExpiration,
                    timeToIdle: OneMinuteExpiration);
                var iface = cache as IAsynchronousCache<string, string>;
                await iface.PutAsync("foo", "bar");

                for (var i = 0; i < 10; i++)
                {
                    await iface.PutAsync(i.ToString(), $"loop{i}");
                    (await iface.GetAsync(i.ToString())).ShouldBe($"loop{i}");
                    (await iface.GetAsync("foo")).ShouldBe("bar");
                    (await iface.GetAsync("baz")).ShouldBe(null);
                }

                cache.Size.ShouldBe(11);
                cache.AccessCount.ShouldBe(30);
                cache.HitCount.ShouldBe(20);
                cache.MissCount.ShouldBe(10);
                cache.GetHitRatio().ShouldBe(0.67, tolerance: 0.01);
            }

            [Fact]
            public async Task Accessing_cache_from_multiple_threads()
            {
                var cache = new DefaultCache<string, string>(
                    "fooCache",
                    timeToLive: OneMinuteExpiration,
                    timeToIdle: OneMinuteExpiration);
                var iface = cache as IAsynchronousCache<string, string>;
                await iface.PutAsync("foo", "bar");

                var tasks = from i in Enumerable.Range(0, 10)
                            select Task.Run(async () =>
                            {
                                await iface.PutAsync(i.ToString(), $"loop{i}");
                                (await iface.GetAsync(i.ToString())).ShouldBe($"loop{i}");
                                (await iface.GetAsync("foo")).ShouldBe("bar");
                                (await iface.GetAsync("baz")).ShouldBe(null);
                            });
                await Task.WhenAll(tasks);

                cache.Size.ShouldBe(11);
                cache.AccessCount.ShouldBe(30);
                cache.HitCount.ShouldBe(20);
                cache.MissCount.ShouldBe(10);
                cache.GetHitRatio().ShouldBe(0.67, tolerance: 0.01);
            }

            [Fact]
            public async Task Cache_access_after_TTL_expiration()
            {
                var cache = new DefaultCache<string, string>(
                    "fooCache",
                    timeToLive: TimeSpan.FromMilliseconds(50),
                    timeToIdle: OneMinuteExpiration);
                var iface = cache as IAsynchronousCache<string, string>;

                await iface.PutAsync("foo", "bar");

                Thread.Sleep(10);
                (await iface.GetAsync("foo")).ShouldBe("bar");

                Thread.Sleep(50);
                (await iface.GetAsync("foo")).ShouldBe(null);
            }

            [Fact]
            public async Task Cache_access_after_TTI_expiration()
            {
                var cache = new DefaultCache<string, string>(
                    "fooCache",
                    timeToLive: OneMinuteExpiration,
                    timeToIdle: TimeSpan.FromMilliseconds(100));
                var iface = cache as IAsynchronousCache<string, string>;

                await iface.PutAsync("foo", "bar");

                Thread.Sleep(50);
                (await iface.GetAsync("foo")).ShouldBe("bar");

                Thread.Sleep(50);
                (await iface.GetAsync("foo")).ShouldBe("bar");

                Thread.Sleep(125);
                (await iface.GetAsync("foo")).ShouldBe(null);
            }
        }
    }
}
