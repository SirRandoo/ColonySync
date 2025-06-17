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
namespace ColonySync.Common.Settings;

/// <summary>
///     Represents the configuration settings required for managing a connection.
/// </summary>
public interface IConnectionSettings
{
    /// <summary>
    ///     Gets the maximum number of retry attempts to perform in case of connection failures.
    /// </summary>
    /// <remarks>
    ///     This property defines the upper limit for the number of reconnection attempts
    ///     a client can make before giving up or raising an error. It is used to manage
    ///     resilience in network communication.
    /// </remarks>
    int MaxRetries { get; init; }

    /// <summary>
    ///     Gets a value indicating whether retries are enabled for the connection.
    ///     When set, this property controls if retry logic should be applied
    ///     during connection attempts or operations that support retries.
    /// </summary>
    int RetryEnabled { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the connection should automatically attempt
    ///     to establish when disconnected or not established initially.
    /// </summary>
    bool AutoConnect { get; init; }

    /// <summary>
    ///     Determines whether a connection message should be sent during the connection process.
    /// </summary>
    /// <remarks>
    ///     This property is used to indicate if an initial connection message will be dispatched
    ///     when establishing a connection. It can be useful for protocols or systems that require
    ///     a handshake or initialization data to be sent when connecting.
    /// </remarks>
    bool SendConnectionMessage { get; init; }

    /// <summary>
    ///     Gets or initializes the OAuth token used for authentication when establishing a connection with an external service
    ///     or API.
    /// </summary>
    string? OAuthToken { get; init; }

    /// <summary>
    ///     An optional client id users can supply.
    /// </summary>
    /// <remarks>
    ///     The primary purpose of this property is to provide an immediate way for 3rd parties to get the chat portion working
    ///     in the event it's decommissioned.
    /// </remarks>
    string? ClientIdOverride { get; init; }

    /// <summary>
    ///     The id of the chat channel to connect to when the connection's established.
    /// </summary>
    string? ChatChannelId { get; init; }
}
