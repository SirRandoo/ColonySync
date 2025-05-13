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

using JetBrains.Annotations;

namespace ColonySync.Common.Settings;

// TODO: These settings should contain a "user type" setting.
// TODO: These settings should contain a "time watching" setting.
// TODO: These settings should contain a "multiplier" setting.
// TODO: These settings should contain a "flat value" setting.
// TODO: These settings should contain a "karma" requirement setting.
// TODO: These settings should contain a "reputation" requirement setting.

/// <summary>Represents configuration settings for point tiers.</summary>
/// <remarks>
///     The settings encapsulated within this record are used to define different tiers related to
///     points within the application.
/// </remarks>
[PublicAPI]
public sealed record PointTierSettings : IVersionedSettings
{
    /// <inheritdoc />
    public int Version { get; init; }
}
