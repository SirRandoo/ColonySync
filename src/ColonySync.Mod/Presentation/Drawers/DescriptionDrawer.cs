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
using UnityEngine;
using Verse;

namespace ColonySync.Mod.Presentation.Drawers;

/// <summary>
///     Provides methods for rendering descriptive text blocks for UI elements in ColonySync. This
///     includes description text, experimental notices, and calculating text dimensions.
/// </summary>
[PublicAPI]
public static class DescriptionDrawer
{
    /// <summary>
    ///     Represents the color used for description text in various UI elements. This color is
    ///     applied when rendering descriptive text to maintain a consistent visual style.
    /// </summary>
    public static readonly Color DescriptionTextColor = new(r: 0.72f, g: 0.72f, b: 0.72f);

    /// <summary>Renders a block of text within a specified region.</summary>
    /// <param name="region">The area where the text will be drawn.</param>
    /// <param name="content">The string content to be displayed.</param>
    /// <param name="color">The color used to render the text.</param>
    public static void DrawTextBlock(Rect region, string content, Color color)
    {
        LabelDrawer.DrawLabel(region, content, color, TextAnchor.UpperLeft, GameFont.Tiny);
    }

    /// <summary>Draws an experimental notice in the specified region with optional custom content.</summary>
    /// <param name="region">The region where the notice will be drawn.</param>
    /// <param name="content">Optional custom content for the notice.</param>
    public static void DrawExperimentalNotice(Rect region, string? content = null)
    {
        if (string.IsNullOrEmpty(content)) content = UxLocale.ExperimentalNotice;

        DrawTextBlock(region, content!, UxColors.RedishPink);
    }

    /// <summary>Draws a description text block within the specified region.</summary>
    /// <param name="region">The rectangular region where the description will be drawn.</param>
    /// <param name="content">The content of the description to be drawn.</param>
    public static void DrawDescription(Rect region, string content)
    {
        DrawTextBlock(region, content, DescriptionTextColor);
    }

    /// <summary>Calculates the height of the text content when rendered within a given width.</summary>
    /// <param name="width">The width constraint for the text.</param>
    /// <param name="content">The text content to be measured.</param>
    /// <returns>The height of the text content within the provided width.</returns>
    public static float GetLineHeight(float width, string content)
    {
        var previous = Text.Font;

        Text.Font = GameFont.Tiny;
        var height = Text.CalcHeight(content, width);
        Text.Font = previous;

        return height;
    }

    /// <summary>
    ///     Calculates the height required to display the experimental notice text within the
    ///     specified width.
    /// </summary>
    /// <param name="width">The width available for rendering the text.</param>
    /// <returns>The height required to display the experimental notice text.</returns>
    public static float GetExperimentalNoticeLineHeight(float width)
    {
        var previous = Text.Font;

        Text.Font = GameFont.Tiny;
        var height = Text.CalcHeight(UxLocale.ExperimentalNotice, width);
        Text.Font = previous;

        return height;
    }

    /// <summary>Calculates the size of a text block given the content, width, and split percentage.</summary>
    /// <param name="content">The text content to be measured.</param>
    /// <param name="width">The total width available for the text block.</param>
    /// <param name="splitPercent">The percentage of the width to be used for the text block.</param>
    /// <returns>A <c>Vector2</c> representing the width and height of the text block.</returns>
    public static Vector2 GetTextBlockSize(string content, float width, float splitPercent)
    {
        var finalWidth = width * splitPercent;

        return new Vector2(finalWidth, GetLineHeight(finalWidth, content));
    }

    /// <summary>Draws a description within the specified listing.</summary>
    /// <param name="listing">The listing in which to draw the description.</param>
    /// <param name="content">The description content to be drawn.</param>
    public static void DrawDescription(this Listing listing, string content)
    {
        DrawDescription(GetTextBlockSize(listing, content), content);
    }

    /// <summary>
    ///     Draws an experimental notice within a specified region. If no content is provided, it uses
    ///     the default experimental notice text from the locale settings.
    /// </summary>
    /// <param name="listing">The <see cref="Listing"/> being used for layout.</param>
    /// <param name="content">
    ///     Optional. Custom content to display as the experimental notice. Defaults to
    ///     null, which uses the locale's default text.
    /// </param>
    public static void DrawExperimentalNotice(this Listing listing, string content)
    {
        DrawExperimentalNotice(GetTextBlockSize(listing, content), content);
    }

    /// <summary>Calculates the size of a text block for rendering purposes.</summary>
    /// <param name="listing">The UI listing element to calculate the text block size for.</param>
    /// <param name="content">The text content to be rendered within the text block.</param>
    /// <returns>A Rect representing the calculated size and position of the text block.</returns>
    private static Rect GetTextBlockSize(Listing listing, string content)
    {
        var size = GetTextBlockSize(content, listing.ColumnWidth, splitPercent: 0.8f);
        var region = listing.GetRect(size.y);

        region.width = size.x;

        return region;
    }
}
