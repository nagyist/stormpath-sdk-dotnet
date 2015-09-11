﻿// <copyright file="JsonNetSerializer.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stormpath.SDK.Serialization;

namespace Stormpath.SDK.Extensions.Serialization.JsonNet
{
    public sealed class JsonNetSerializer : IJsonSerializer
    {
        private readonly JsonSerializerSettings serializerSettings;

        public JsonNetSerializer()
        {
            this.serializerSettings = new JsonSerializerSettings();
            this.serializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
            this.serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
        }

        string IJsonSerializer.Serialize(IDictionary<string, object> map)
        {
            var serialized = JsonConvert.SerializeObject(map);

            return serialized;
        }

        IDictionary<string, object> IJsonSerializer.Deserialize(string json)
        {
            var deserializedMap = (JObject)JsonConvert.DeserializeObject(json, this.serializerSettings);
            var sanitizedMap = this.Sanitize(deserializedMap);

            return sanitizedMap;
        }

        /// <summary>
        /// JSON.NET deserializes everything into nested JObjects. We want IDictionaries all the way down.
        /// </summary>
        /// <param name="map">Deserialized <see cref="JObject"/> from JSON.NET</param>
        /// <returns><see cref="IDictionary{string, object}"/> of primitive items, and embedded objects as nested <see cref="IDictionary{string, object}"/></returns>
        private IDictionary<string, object> Sanitize(JObject map)
        {
            var result = new Dictionary<string, object>(map.Count);

            foreach (var prop in map.Properties())
            {
                var name = prop.Name;
                object value = null;

                switch (prop.Value.Type)
                {
                    case JTokenType.Array:
                        var nested = new List<IDictionary<string, object>>();
                        foreach (var child in prop.Value.Children())
                        {
                            nested.Add(this.Sanitize((JObject)child));
                        }

                        value = nested;
                        break;

                    case JTokenType.Object:
                        value = this.Sanitize((JObject)prop.Value);
                        break;

                    case JTokenType.Date:
                        value = prop.Value.ToObject<DateTimeOffset>();
                        break;

                    case JTokenType.Integer:
                        value = int.Parse(prop.Value.ToString());
                        break;

                    case JTokenType.Boolean:
                        value = bool.Parse(prop.Value.ToString());
                        break;

                    case JTokenType.Null:
                        value = null;
                        break;

                    default:
                        value = prop.Value.ToString();
                        break;
                }

                result.Add(name, value);
            }

            return result;
        }
    }
}