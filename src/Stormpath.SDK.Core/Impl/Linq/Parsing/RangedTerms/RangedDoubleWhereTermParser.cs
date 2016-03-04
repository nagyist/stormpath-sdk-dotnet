﻿// <copyright file="RangedDoubleWhereTermParser.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Linq;

namespace Stormpath.SDK.Impl.Linq.Parsing.RangedTerms
{
    internal sealed class RangedDoubleWhereTermParser : AbstractRangedWhereTermParser<SpecifiedPrecisionToken>
    {
        protected override SpecifiedPrecisionToken? Coerce(object value)
        {
            try
            {
                if (value is SpecifiedPrecisionToken)
                {
                    return (SpecifiedPrecisionToken)value;
                }
                else
                {
                    return new SpecifiedPrecisionToken(Convert.ToDouble(value));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not add ranged integer term. See the inner exception for details.", ex);
            }
        }

        protected override string Format(SpecifiedPrecisionToken value)
            => value.ToString();
    }
}