// MIT License
//
// Copyright (c) 2023 SirRandoo
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

namespace ColonySync.Common.Interfaces;

/// <summary>Enum that defines various features a streaming platform can support.</summary>
/// <remarks>
///     This enumeration uses the [Flags] attribute to allow bitwise combinations of its member
///     values.
/// </remarks>
[Flags]
public enum PlatformFeatures
{
    /// <summary>Represents a state where no platform features are enabled.</summary>
    None = 0,

    /// <summary>Indicates that the platform supports creating and managing polls.</summary>
    Polls = 1,

    /// <summary>Supports the ability to manage or participate in raids on supported platforms.</summary>
    Raids = 2,

    /// <summary>Represents the feature enabling user redemptions within the platform.</summary>
    Redemptions = 4,

    /// <summary>
    ///     Represents the extensions feature in a platform, which allows for the addition of custom
    ///     functionalities or modifications.
    /// </summary>
    Extensions = 8,

    /// <summary>
    ///     The Predictions flag indicates that the platform supports prediction features, allowing
    ///     users to participate in interactive prediction activities.
    /// </summary>
    Predictions = 16,

    /// <summary>Represents the feature that allows platforms to manage and handle subscriptions.</summary>
    Subscriptions = 32,

    /// <summary>
    ///     Represents the capability of a platform to support gift subscriptions. This feature allows
    ///     users to purchase subscriptions for other users, enhancing community engagement and providing
    ///     streamers an additional revenue avenue.
    /// </summary>
    GiftSubscriptions = 64
}

/// <summary>
///     Represents a platform with identifiable properties and features including icon data and specific platform
///     features.
/// </summary>
public interface IPlatform : IIdentifiable
{
    /// <summary>
    ///     Gets or sets the icon data, which is represented as a byte array.
    /// </summary>
    byte[] IconData { get; set; }

    /// <summary>
    ///     Gets or sets the features supported by the platform, represented as a combination of
    ///     <see cref="PlatformFeatures" /> flags.
    /// </summary>
    PlatformFeatures Features { get; }
}