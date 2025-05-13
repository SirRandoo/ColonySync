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

/// <summary>Encapsulates various configurable settings for a store in ColonySync.</summary>
/// <remarks>
///     StoreSettings is a sealed record that consolidates the configuration options related to a store, such as the
///     store's URL,
///     restrictions on purchases, and settings for enabling purchase confirmations or enabling purchasable buildings.
///     As an implementation of IVersionedSettings, it ensures compatibility by tracking a version number for the settings.
/// </remarks>
[PublicAPI]
public sealed record StoreSettings : IVersionedSettings
{
    /// <summary>Specifies the URI associated with the store.</summary>
    public Uri? StoreLink { get; init; }

    /// <summary>Specifies the minimum price required to complete a purchase in the store.</summary>
    /// <remarks>
    ///     This property enforces a threshold to ensure that purchases meet or exceed a specified value.
    ///     It is intended to prevent transactions below a certain monetary level.
    /// </remarks>
    public int MinimumPurchasePrice { get; init; } = 50;

    /// <summary>Indicates whether purchase confirmations are required in the store.</summary>
    public bool PurchaseConfirmations { get; init; } = true;

    /// <summary>Indicates whether buildings can be purchased in the store.</summary>
    public bool BuildingsPurchasable { get; init; }

    /// <inheritdoc />
    public int Version { get; init; } = 1;
}