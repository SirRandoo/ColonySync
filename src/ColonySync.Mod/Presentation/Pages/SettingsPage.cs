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

using ColonySync.Common.Enums;
using UnityEngine;
using Verse;

namespace ColonySync.Mod.Presentation.Pages;

/// <summary>Represents an IPage implementation suitable for displaying settings.</summary>
public abstract class SettingsPage : IPage
{
    /// <summary>Specifies the number of lines that embedded settings panels should occupy.</summary>
    protected const int PanelLineSpan = 6;

    /// <summary>Contains the names of each unit of time.</summary>
    protected static readonly string[] UnitOfTimeNames = UnitOfTime.GetNames();

    /// <inheritdoc />
    public abstract void Draw(Rect region);

    /// <summary>Creates a <see cref="Listing" /> instance suitable for rapidly layout out settings.</summary>
    /// <param name="region">The region of the screen the listing is operating in.</param>
    /// <returns>A <see cref="Listing" /> instance configured for the specified region.</returns>
    protected static Listing CreateListing(Rect region)
    {
        return new Listing_Standard(GameFont.Small)
        {
            ColumnWidth = region.width, maxOneColumn = true
        };
    }
}