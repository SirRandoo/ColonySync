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

namespace ColonySync.Common.Interfaces;

/// <summary>
///     Represents a product item that can be purchased by viewers in a chat. In addition to the
///     basic product properties, it can have specific attributes such as weapon properties, apparel
///     properties, and material properties.
/// </summary>
public interface IItemProduct : IProduct
{
    /// <summary>Gets or sets the weapon properties associated with the item.</summary>
    /// <remarks>
    ///     This property implements the <see cref="IWeaponProperties" /> interface and provides
    ///     detailed characteristics about a weapon that can be equipped by a player. The properties may
    ///     include information such as weapon type, equippable status, and other weapon-specific
    ///     attributes.
    /// </remarks>
    IWeaponProperties? WeaponProperties { get; init; }

    /// <summary>Gets or sets the properties specific to apparel items.</summary>
    IApparelProperties? ApparelProperties { get; init; }

    /// <summary>Defines the material properties associated with an item.</summary>
    IMaterialProperties? MaterialProperties { get; init; }
}