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

using ColonySync.Common.Settings;

namespace ColonySync.Plugins.Twitch;

/// <summary>
///     <para>
///         A class representing settings for configuring interactions with the Twitch platform.
///     </para>
///     <para>
///         This class provides the implementation for versioned settings and encapsulates
///         configuration options such as the bot token, target channel, and connection behaviors.
///     </para>
/// </summary>
public sealed class TwitchSettings : IVersionedSettings, IConnectionSettings
{
    private const int LatestVersion = 1;

    /// <inheritdoc />
    public string? ChatChannelId { get; init; }

    /// <inheritdoc />
    public bool SendConnectionMessage { get; init; }

    /// <inheritdoc />
    public int RetryEnabled { get; init; }

    /// <summary>
    ///     Indicates whether the bot should automatically attempt to connect to the Twitch platform
    ///     upon initialization without requiring a manual trigger.
    /// </summary>
    public bool AutoConnect { get; init; }

    /// <inheritdoc />
    public string? OAuthToken { get; init; }

    /// <inheritdoc />
    public string? ClientIdOverride { get; init; }

    /// <summary>
    ///     Specifies the maximum number of retry attempts the system should make when a failure occurs during interactions
    ///     with the Twitch platform.
    /// </summary>
    public int MaxRetries { get; init; } = 5;

    /// <inheritdoc />
    public int Version { get; init; } = LatestVersion;
}
