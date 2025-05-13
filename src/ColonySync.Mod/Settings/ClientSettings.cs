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

using ColonySync.Common.Entities;
using ColonySync.Common.Settings;
using JetBrains.Annotations;

namespace ColonySync.Mod.Settings;

/// <summary>
///     A class that encapsulates settings specific to the client side of the mod.
/// </summary>
/// <remarks>
///     The ClientSettings class is responsible for defining and maintaining configuration details
///     that are critical for client-side functionality within the mod. These settings are versioned
///     to ensure compatibility and can be customized based on the user’s preferences or requirements.
/// </remarks>
[PublicAPI]
public sealed record ClientSettings : IVersionedSettings
{
    /// <summary>
    ///     Specifies the on-screen position of the poll window in the client settings.
    /// </summary>
    public Position2D PollWindowPosition { get; init; }

    /// <summary>Indicates whether the GizmoPuff feature is enabled.</summary>
    public bool GizmoPuff { get; init; } = true;

    /// <inheritdoc />
    public int Version { get; init; } = 1;
}
