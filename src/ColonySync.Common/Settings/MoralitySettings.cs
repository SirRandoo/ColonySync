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

using JetBrains.Annotations;

namespace ColonySync.Common.Settings;

/// <summary>
///     Encapsulates the configuration settings for morality within the ColonySync system, which include the management of
///     karma, reputation, and related range constraints.
/// </summary>
[PublicAPI]
public sealed record MoralitySettings : IVersionedSettings
{
    /// <summary>
    ///     Represents the allowable range for alignment values, defining the minimum and maximum limits.
    /// </summary>
    public ShortRange AlignmentRange { get; init; } = new();

    /// <summary>
    ///     Indicates whether the reputation system is enabled.
    /// </summary>
    public bool IsReputationEnabled { get; init; } = true;

    /// <summary>
    ///     The initial alignment or karma value assigned to a user or entity at the start.
    /// </summary>
    public short StartingAlignment { get; init; } = 10;

    /// <summary>
    ///     Indicates whether the karma system is enabled.
    /// </summary>
    public bool IsAlignmentEnabled { get; init; } = true;

    /// <summary>
    ///     Indicates whether momentum-based effects or mechanics are enabled in the morality settings.
    /// </summary>
    public bool IsMomentumEnabled { get; init; } = true;

    /// <inheritdoc />
    public int Version { get; init; } = 1;
}
