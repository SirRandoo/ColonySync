// MIT License
// 
// Copyright (c) 2025 sirrandoo
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
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ColonySync.Common.Settings;

/// <summary>
///     Represents configuration settings for point-related features and functionality within the system.
/// </summary>
[PublicAPI]
public sealed record PointSettings : IVersionedSettings
{
    /// <summary>
    ///     Represents the configuration for decay of points over time.
    /// </summary>
    public List<PointDecaySettings> PointDecaySettings { get; init; } = [];

    /// <summary>
    ///     Indicates whether the point system includes tiered point levels.
    /// </summary>
    public bool HasPointTiers { get; init; }

    /// <summary>
    ///     Configuration for point tier settings, defining rules and requirements for different point levels.
    /// </summary>
    public List<PointTierSettings>? PointTierSettings { get; init; }

    /// <summary>
    ///     Indicates whether rewards are enabled for the points system.
    /// </summary>
    public bool HasRewards { get; init; }

    /// <summary>Configuration settings for daily rewards within the system.</summary>
    public List<DailyRewardSettings>? DailyRewardSettings { get; init; }

    /// <summary>Indicates whether point decay functionality is enabled.</summary>
    public bool HasPointDecay { get; init; }

    /// <summary>
    ///     Indicates whether user participation is required for earning points or rewards.
    /// </summary>
    public bool ParticipationRequired { get; init; } = true;

    /// <summary>
    ///     Specifies the numerical value of the reward given at each interval.
    /// </summary>
    public int RewardAmount { get; init; } = 100;

    /// <summary>Defines the time interval between reward distributions.</summary>
    public TimeSpan RewardInterval { get; init; } = TimeSpan.FromMinutes(5);

    /// <summary>
    ///     The initial point balance assigned to a user or account upon creation or initialization.
    /// </summary>
    public int StartingBalance { get; init; } = 50;

    /// <summary>
    ///     Indicates whether points are actively being distributed.
    /// </summary>
    public bool IsDistributing { get; init; } = true;

    /// <summary>
    ///     Indicates whether the points are unlimited and cannot be depleted.
    /// </summary>
    public bool InfinitePoints { get; init; }

    /// <inheritdoc />
    public int Version { get; init; } = 1;
}
