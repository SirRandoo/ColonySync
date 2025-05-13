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
using ColonySync.Common.Enums;
using JetBrains.Annotations;

namespace ColonySync.Common.Settings;

/// <summary>Enumerates the different types of rewards available for viewers on a daily basis.</summary>
[PublicAPI]
public enum DailyRewardType
{
    /// <summary>Represents a reward that contains a randomized item from a loot table.</summary>
    MysteryBox,

    /// <summary>Represents a reward that grants a fixed number of points to the viewer.</summary>
    FixedPoints
}

/// <summary>
///     Represents the configurable base settings for different types of daily rewards.
/// </summary>
[PublicAPI]
public abstract record DailyRewardSettings : IVersionedSettings
{
    /// <summary>
    ///     Gets the type of reward available for daily distribution.
    /// </summary>
    public DailyRewardType Type { get; init; }

    /// <summary>
    ///     The probability or likelihood associated with obtaining the configured daily reward.
    /// </summary>
    public float Chance { get; init; }

    /// <summary>
    ///     Indicates whether a stream-based threshold is enabled for the reward settings.
    /// </summary>
    public bool HasStreamThreshold { get; init; }

    /// <summary>
    ///     Defines the stream threshold required to qualify for a daily reward.
    ///     This value typically represents a specific numerical limit or condition
    ///     associated with streaming activity.
    /// </summary>
    public int StreamThreshold { get; init; }

    /// <summary>
    ///     Indicates whether a time threshold is applicable for the daily reward.
    /// </summary>
    public bool HasTimeThreshold { get; init; }

    /// <summary>Defines the time threshold for daily rewards to become eligible.</summary>
    public TimeSpan TimeThreshold { get; init; }

    /// <summary>
    ///     Indicates whether specific roles are required to access or qualify for the daily reward.
    /// </summary>
    public bool HasRequiredRoles { get; init; }

    /// <summary>
    ///     Determines whether at least one of the roles specified in the <see cref="RequiredRoles" /> property
    ///     is sufficient to fulfill the role requirement for the daily reward.
    /// </summary>
    public bool RequireAnyRoles { get; init; }

    /// <summary>
    ///     Specifies the user roles required to be eligible for the daily reward.
    /// </summary>
    public UserRoles RequiredRoles { get; init; }

    /// <inheritdoc />
    public int Version { get; init; } = 1;
}

/// <summary>Represents the settings for daily rewards involving a mystery box.</summary>
[PublicAPI]
public sealed record MysteryBoxDailyRewardSetting : DailyRewardSettings
{
    /// <summary>
    ///     Gets the range configuration for the daily reward.
    /// </summary>
    public IntRange Range { get; init; }
}

/// <summary>
///     Represents the settings for a fixed points daily reward in the application.
/// </summary>
[PublicAPI]
public sealed record FixedPointsDailyRewardSetting : DailyRewardSettings
{
    /// <summary>
    ///     Gets the fixed number of points awarded as a daily reward.
    /// </summary>
    public int Points { get; init; }
}
