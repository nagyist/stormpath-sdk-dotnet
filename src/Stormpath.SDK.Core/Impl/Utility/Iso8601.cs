﻿// <copyright file="Iso8601.cs" company="Stormpath, Inc.">
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

using System;
using System.Globalization;

namespace Stormpath.SDK.Impl.Utility
{
    internal static class Iso8601
    {
        private static readonly string FormatWithoutOffset = "yyyy-MM-ddTHH:mm:ss.fff";

        private static readonly string ParsePatternWithSeparators = "yyyy-MM-dd'T'HH:mm:ss.FFFK";
        private static readonly string ParsePatternWithoutSeparators = "yyyyMMdd'T'HHmmssFFFK";

        public static string Format(DateTimeOffset dto, bool convertToUtc = true)
        {
            var offsetComponent = "zzz";

            if (convertToUtc || dto.Offset == TimeSpan.Zero)
            {
                offsetComponent = "Z";
            }

            var outputFormat = $"{FormatWithoutOffset}{offsetComponent}";

            return convertToUtc
                ? dto.UtcDateTime.ToString(outputFormat, CultureInfo.InvariantCulture)
                : dto.ToString(outputFormat, CultureInfo.InvariantCulture);
        }

        public static DateTimeOffset Parse(string iso8601String)
        {
            return DateTimeOffset.ParseExact(
                iso8601String,
                new string[] { ParsePatternWithSeparators, ParsePatternWithoutSeparators },
                CultureInfo.InvariantCulture,
                DateTimeStyles.None);
        }
    }
}
