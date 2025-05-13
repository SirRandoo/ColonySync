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
using JetBrains.Annotations;

namespace ColonySync.Common.Settings;

/// <summary>
///     Represents the settings for configuring polls within the application. This class contains options
///     that influence the behavior and appearance of polls, including but not limited to duration,
///     animation preferences, and text size.
/// </summary>
[PublicAPI]
public sealed record PollSettings : IVersionedSettings
{
    /// <summary>Specifies the duration for which the poll remains active.</summary>
    public TimeSpan Duration { get; init; } = TimeSpan.FromMinutes(2);

    /// <summary>The maximum number of options allowed in the poll.</summary>
    public int MaximumOptions { get; init; } = 4;

    /// <summary>Indicates whether to prefer native polls.</summary>
    public bool PreferNativePolls { get; init; }

    /// <summary>Indicates whether to display the poll dialog when initiating a poll.</summary>
    public bool ShowPollDialog { get; init; } = true;

    /// <summary>Indicates whether to use large text for poll options.</summary>
    public bool UseLargeText { get; init; }

    /// <summary>Indicates whether to animate vote tallies.</summary>
    public bool AnimateVotes { get; init; } = true;

    /// <summary>Indicates whether to generate random polls.</summary>
    public bool GenerateRandomPolls { get; init; } = true;

    /// <summary>Indicates whether to post poll options in the chat.</summary>
    public bool PostOptionsInChat { get; init; } = true;

    /// <inheritdoc />
    public int Version { get; init; } = 1;
}
