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

using ColonySync.Common.Enums;
using JetBrains.Annotations;

namespace ColonySync.Common.Settings;

/// <summary>
///     Represents the settings configuration for a pawn in ColonySync. This class contains various
///     properties to control pawn behavior, such as pooling, vacationing, and emergency work crisis handling.
/// </summary>
/// <param name="Version">The version of the settings configuration.</param>
/// <param name="Pooling">Indicates whether pooling is enabled for the pawn.</param>
/// <param name="PoolingRestrictions">Specifies any user role restrictions on pooling.</param>
/// <param name="Vacationing">Indicates whether vacationing is enabled for the pawn.</param>
/// <param name="TotalOverworkedPawns">Specifies the total number of overworked pawns allowed.</param>
/// <param name="EmergencyWorkCrisis">Indicates whether the pawn should handle emergency work crises.</param>
[PublicAPI]
public sealed record PawnSettings : IVersionedSettings
{
    /// <summary>Indicates whether pooling is enabled for the pawn.</summary>
    public bool Pooling { get; init; } = true;

    /// <summary>Specifies any user role restrictions on pooling.</summary>
    public UserRoles PoolingRestrictions { get; init; } = UserRoles.None;

    /// <summary>Indicates whether vacationing is enabled for the pawn.</summary>
    public bool Vacationing { get; init; } = true;

    /// <summary>Specifies the total number of overworked pawns allowed.</summary>
    public short TotalOverworkedPawns { get; init; } = 1;

    /// <summary>Indicates whether the pawn should handle emergency work crises.</summary>
    public bool EmergencyWorkCrisis { get; init; } = true;

    /// <inheritdoc />
    public int Version { get; init; } = 1;
}
