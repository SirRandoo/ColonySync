// MIT License
//
// Copyright (c) 2024 SirRandoo
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using ColonySync.Common.Interfaces;
using JetBrains.Annotations;

namespace ColonySync.Common.Settings;

/// <summary>Represents the settings for point decay in the system.</summary>
/// <param name="Version">The version number of the settings.</param>
/// <param name="Id">The unique identifier for the decay setting.</param>
/// <param name="Name">The name of the decay setting.</param>
/// <param name="DecayPercent">The percentage by which points decay over the period.</param>
/// <param name="FixedAmount">The fixed amount by which points decay over the period.</param>
/// <param name="Period">The time period over which the points will decay.</param>
[PublicAPI]
public sealed record PointDecaySettings : IVersionedSettings, IIdentifiable
{
    /// <summary>The unique identifier for the decay setting.</summary>
    public string Id { get; init; } = Guid.NewGuid().ToString();

    /// <summary>The name of the decay setting.</summary>
    public string Name { get; init; } = "Point Decay";

    /// <summary>The percentage by which points decay over the period.</summary>
    public float DecayPercent { get; init; }

    /// <summary>The fixed amount by which points decay over the period.</summary>
    public int FixedAmount { get; init; }

    /// <summary>The time period over which the points will decay.</summary>
    public TimeSpan Period { get; init; }

    /// <inheritdoc />
    public int Version { get; init; } = 1;
}
