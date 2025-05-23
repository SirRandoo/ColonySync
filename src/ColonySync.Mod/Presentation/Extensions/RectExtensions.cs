﻿// MIT License
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

using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;

namespace ColonySync.Mod.Presentation.Extensions;

/// <summary>Provides extension methods for manipulating Rect structures in various ways.</summary>
[PublicAPI]
public static class RectExtensions
{
    /// <summary>Shifts a <see cref="Rect" /> in the specified direction.</summary>
    /// <param name="region">The region to shift</param>
    /// <param name="direction">The direction to shift the region to</param>
    /// <param name="padding">The amount of padding to add to the shifted region</param>
    /// <returns>The shifted region</returns>
    public static Rect Shift(this Rect region, Direction8Way direction, float padding = 5f)
    {
        return direction switch
        {
            Direction8Way.North => new Rect(region.x, region.y - region.height - padding, region.width, region.height),
            Direction8Way.NorthEast => new Rect(
                region.x + region.width + padding, region.y - region.height - padding, region.width, region.height
            ),
            Direction8Way.East => new Rect(region.x + region.width + padding, region.y, region.width, region.height),
            Direction8Way.SouthEast => new Rect(
                region.x + region.width + padding, region.y + region.height + padding, region.width, region.height
            ),
            Direction8Way.South => new Rect(region.x, region.y + region.height + padding, region.width, region.height),
            Direction8Way.SouthWest => new Rect(
                region.x - region.width - padding, region.y + region.height + padding, region.width, region.height
            ),
            Direction8Way.West => new Rect(region.x - region.width - padding, region.y, region.width, region.height),
            Direction8Way.NorthWest => new Rect(
                region.x - region.width - padding, region.y - region.height - padding, region.width, region.height
            ),
            _ => region
        };
    }

    /// <summary>Determines whether a given region is visible in a scroll view.</summary>
    /// <param name="region">The region in question</param>
    /// <param name="scrollRect">The visible region of the scroll view</param>
    /// <param name="scrollPos">The current scroll position of the scroll bar</param>
    /// <returns>Whether the region is visible</returns>
    public static bool IsVisible(this Rect region, Rect scrollRect, Vector2 scrollPos)
    {
        return (region.y >= scrollPos.y || region.y + region.height - 1f >= scrollPos.y)
               && region.y <= scrollPos.y + scrollRect.height;
    }

    /// <summary>Splits a <see cref="Rect" /> in two.</summary>
    /// <param name="region">The rect to split</param>
    /// <param name="percent">A percent indicating how big the left side of the split should be relative to the region's width</param>
    /// <returns>A tuple containing the left and right sides of the current line rect</returns>
    public static (Rect leftRegion, Rect rightRegion) Split(this Rect region, float percent = 0.8f)
    {
        var left = new Rect(region.x, region.y, Mathf.FloorToInt(region.width * percent - 2f), region.height);

        return (left, new Rect(left.x + left.width + 2f, left.y, region.width - left.width - 2f, left.height));
    }

    /// <summary>Splits a <see cref="Rect" /> in two.</summary>
    /// <param name="listing">The <see cref="Listing" /> object to use</param>
    /// <param name="percent">A percent indicating how big the left side of the split should be relative to the region's width</param>
    /// <returns>A tuple containing the left and right sides of the current line rect</returns>
    public static (Rect leftRegion, Rect rightRegion) Split(this Listing listing, float percent = 0.8f)
    {
        var region = listing.GetRect(UiConstants.LineHeight);
        var leftRegion = new Rect(region.x, region.y, region.width * percent, region.height);
        region.width -= leftRegion.width;
        region.x = leftRegion.x + leftRegion.width;

        return (leftRegion, region);
    }

    /// <summary>Splits the given region into two <see cref="Rect" />s.</summary>
    /// <param name="x">The x coordinate of the region being split.</param>
    /// <param name="y">The y coordinate of the region being split.</param>
    /// <param name="width">The width of the region being split.</param>
    /// <param name="height">The height of the region being split.</param>
    /// <param name="percent">A percent indicating how big the left side of the split should be relative to the region's width</param>
    /// <returns>A tuple containing the left and right sides of the split region.</returns>
    public static (Rect leftRegion, Rect rightRegion) Split(
        float x,
        float y,
        float width,
        float height,
        float percent = 0.8f
    )
    {
        var left = new Rect(x, y, Mathf.FloorToInt(width * percent), height);
        var right = new Rect(x + left.width, y, width - left.width, height);

        return (left, right);
    }

    /// <summary>Trims a <see cref="Rect" /> by the specified amount in the specified direction.</summary>
    /// <param name="region">The region to trim</param>
    /// <param name="direction">The direction to trim</param>
    /// <param name="amount">The amount to trim</param>
    /// <returns>The trimmed rect</returns>
    public static Rect Trim(this Rect region, Direction8Way direction, float amount)
    {
        return direction switch
        {
            Direction8Way.North => new Rect(region.x, region.y + amount, region.width, region.height - amount),
            Direction8Way.NorthEast => new Rect(
                region.x, region.y + amount, region.width - amount, region.height - amount
            ),
            Direction8Way.East => new Rect(region.x, region.y, region.width - amount, region.height),
            Direction8Way.SouthEast => new Rect(region.x, region.y, region.width - amount, region.height - amount),
            Direction8Way.South => new Rect(region.x, region.y, region.width, region.height - amount),
            Direction8Way.SouthWest => new Rect(
                region.x + amount, region.y, region.width - amount, region.height - amount
            ),
            Direction8Way.West => new Rect(region.x + amount, region.y, region.width - amount, region.height),
            Direction8Way.NorthWest => new Rect(
                region.x + amount, region.y + amount, region.width - amount, region.height - amount
            ),
            _ => region
        };
    }

    /// <summary>Shrinks and centers a deconstructed <see cref="Rect" /> for use in drawing icons.</summary>
    /// <param name="x">The X position of a region</param>
    /// <param name="y">The Y position of a region</param>
    /// <param name="width">The width of the given region</param>
    /// <param name="height">The height of the given region</param>
    /// <param name="margin">The amount of margins to add to the final <see cref="Rect" /></param>
    /// <returns>A scaled, centered <see cref="Rect" /> for drawing icons</returns>
    public static Rect IconRect(float x, float y, float width, float height, float margin = 2f)
    {
        var shortest = Mathf.Min(width, height);
        float halfShortest = Mathf.FloorToInt(shortest / 2f);
        float halfWidth = Mathf.FloorToInt(width / 2f);
        float halfHeight = Mathf.FloorToInt(height / 2f);

        return new Rect(
            Mathf.Clamp(x + halfWidth - halfShortest, x, x + width) + margin,
            Mathf.Clamp(y + halfHeight - halfShortest, y, y + height) + margin,
            shortest - margin * 2f,
            shortest - margin * 2f
        );
    }

    /// <summary>Trims a <see cref="Rect" /> to a square icon, with optional margin.</summary>
    /// <param name="region">The reference to the region being trimmed.</param>
    /// <param name="margin">The margin to trim from the region.</param>
    /// <returns>The trimmed region, adjusted for the icon size and margin.</returns>
    public static Rect TrimToIconRect(this Rect region, float margin = 2f)
    {
        region.x = region.x + region.width - region.height;
        region.width = region.height;

        var shortest = Mathf.Min(region.width, region.height);
        float halfShortest = Mathf.FloorToInt(shortest / 2f);
        float halfWidth = Mathf.FloorToInt(region.width / 2f);
        float halfHeight = Mathf.FloorToInt(region.height / 2f);

        region.x = Mathf.Clamp(region.x + halfWidth - halfShortest, region.x, region.x + region.width) + margin;
        region.y = Mathf.Clamp(region.y + halfHeight - halfShortest, region.y, region.y + region.height) + margin;
        region.width = shortest - margin * 2f;
        region.height = shortest - margin * 2f;

        return region;
    }
}
