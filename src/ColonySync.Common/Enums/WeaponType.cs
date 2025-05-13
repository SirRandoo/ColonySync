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

using NetEscapades.EnumGenerators;

namespace ColonySync.Common.Enums;

/// <summary>Specifies the general categories of weapons.</summary>
[EnumExtensions]
public enum WeaponType
{
    /// <summary>Indicates that no weapon type is specified.</summary>
    None,

    /// <summary>
    ///     Represents a weapon that is used in close combat, typically involving physical strikes
    ///     such as with swords, axes, or fists.
    /// </summary>
    Melee,

    /// <summary>
    ///     Represents weapons that can attack targets at a distance. This includes firearms, bows,
    ///     and other long-range weaponry.
    /// </summary>
    Ranged
}