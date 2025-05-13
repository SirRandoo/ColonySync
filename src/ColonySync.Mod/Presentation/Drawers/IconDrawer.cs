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

using ColonySync.Mod.Presentation.Extensions;
using JetBrains.Annotations;
using UnityEngine;
using Verse;

namespace ColonySync.Mod.Presentation.Drawers;

/// <summary>Provides methods for drawing icons in various regions in the user interface.</summary>
[PublicAPI]
public static class IconDrawer
{
    /// <summary>Draws the specified texture in the color given.</summary>
    /// <param name="region">The region to draw the texture in</param>
    /// <param name="icon">The texture to draw</param>
    /// <param name="color">The color to draw the texture</param>
    /// <remarks>
    ///     This method doesn't recolor the texture given to the color specified; it changes the value
    ///     of <see cref="GUI.color" /> to the color specified and lets Unity handle the recoloring.
    /// </remarks>
    public static void DrawIcon(Rect region, Texture2D icon, Color? color)
    {
        region = RectExtensions.IconRect(region.x, region.y, region.width, region.height);

        var old = GUI.color;

        GUI.color = color ?? Color.white;
        GUI.DrawTexture(region, icon);
        GUI.color = old;
    }

    /// <summary>An internal method for creating a Rect suitable for drawing "field icons" in.</summary>
    /// <param name="parentRegion">The region of the field the icon is being drawn over</param>
    /// <param name="offset">An optional number indicating how many slots to offset the icon</param>
    /// <returns>The Rect to draw the field icon in</returns>
    internal static Rect GetFieldIconRect(Rect parentRegion, int offset = 0)
    {
        return RectExtensions.IconRect(
            parentRegion.x + parentRegion.width - parentRegion.height * (offset + 1),
            parentRegion.y,
            parentRegion.height,
            parentRegion.height,
            Mathf.CeilToInt(parentRegion.height * 0.1f)
        );
    }

    /// <summary>Draws an icon over an input field.</summary>
    /// <param name="parentRegion">The region of the field the icon is being drawn over</param>
    /// <param name="icon">A character to be used as the icon</param>
    /// <param name="tooltip">An optional tooltip for the icon</param>
    /// <param name="offset">An optional number indicated how many slots to offset the icon</param>
    public static void DrawFieldIcon(Rect parentRegion, char icon, string? tooltip = null, int offset = 0)
    {
        var region = GetFieldIconRect(parentRegion, offset);
        LabelDrawer.DrawLabel(region, icon.ToString(), TextAnchor.MiddleCenter);
        TooltipHandler.TipRegion(region, tooltip);
    }

    /// <summary>Draws a field icon within a specified region.</summary>
    /// <param name="parentRegion">The region of the field the icon is being drawn over</param>
    /// <param name="icon">A character to be used as the icon</param>
    /// <param name="tooltip">An optional tooltip for the icon</param>
    /// <param name="offset">An optional number indicating how many slots to offset the icon</param>
    public static void DrawFieldIcon(Rect parentRegion, string icon, string? tooltip = null, int offset = 0)
    {
        var region = GetFieldIconRect(parentRegion, offset);
        LabelDrawer.DrawLabel(region, icon, TextAnchor.MiddleCenter);
        TooltipHandler.TipRegion(region, tooltip);
    }

    /// <summary>Draws an icon over an input field.</summary>
    /// <param name="parentRegion">The region of the field the button is being drawn over</param>
    /// <param name="icon">A texture to be drawn as the icon</param>
    /// <param name="tooltip">An optional tooltip for the icon</param>
    /// <param name="offset">An optional number indicating how many slots to offset the icon</param>
    public static void DrawFieldIcon(Rect parentRegion, Texture2D icon, string? tooltip = null, int offset = 0)
    {
        var region = GetFieldIconRect(parentRegion, offset);
        GUI.DrawTexture(region, icon);
        TooltipHandler.TipRegion(region, tooltip);
    }

    /// <summary>
    ///     Draws an experimental icon cutout in the specified region and updates the region to
    ///     account for the space taken by the icon.
    /// </summary>
    /// <param name="region">
    ///     The region to draw the icon cutout in. The region's dimensions will be
    ///     adjusted after drawing the icon.
    /// </param>
    /// <remarks>
    ///     This method draws an icon indicating an experimental feature and shifts the given region
    ///     to the right to account for the icon's space. Additionally, if the mouse is over the icon
    ///     cutout, a tooltip for the experimental notice will be displayed.
    /// </remarks>
    public static void DrawExperimentalIconCutout(ref Rect region)
    {
        var cutout = RectExtensions.IconRect(region.x, region.y, region.height, region.height, margin: 6f);

        region.x += region.height;
        region.width -= region.height;

        DrawIcon(cutout, Icons.Get(IconCategory.Solid, "TriangleExclamation"), UxColors.RedishPink);

        if (Mouse.IsOver(cutout)) TooltipHandler.TipRegion(cutout, UxLocale.ExperimentalNoticeColored);
    }

    /// <summary>Draws a sort indicator based on the given sort order.</summary>
    /// <param name="parentRegion">The region to draw the sort indicator in.</param>
    /// <param name="order">The sorting order to indicate (None, Ascending, or Descending).</param>
    public static void DrawSortIndicator(Rect parentRegion, SortOrder order)
    {
        var region = RectExtensions.IconRect(
            parentRegion.x + parentRegion.width - parentRegion.height + 3f,
            parentRegion.y + 8f,
            parentRegion.height - 9f,
            parentRegion.height - 16f,
            margin: 0f
        );

        switch (order)
        {
            case SortOrder.Ascending:
                GUI.DrawTexture(region, Icons.Get(IconCategory.Solid, "SortUp"));

                return;
            case SortOrder.Descending:
                GUI.DrawTexture(region, Icons.Get(IconCategory.Solid, "SortDown"));

                return;
            case SortOrder.None:
                GUI.DrawTexture(region, Icons.Get(IconCategory.Solid, "Sort"));

                return;
        }
    }

    /// <summary>
    ///     Draws the specified ThingDef in the given region, optionally displaying an info card when
    ///     clicked.
    /// </summary>
    /// <param name="region">The region to draw the ThingDef in.</param>
    /// <param name="def">The ThingDef to draw.</param>
    /// <param name="labelOverride">Optional label to use instead of the ThingDef's default label.</param>
    /// <param name="infoCard">Specifies if an info card should be shown when the region is clicked.</param>
    /// <remarks>
    ///     Draws the icon of the ThingDef and its label within the specified region. If infoCard is
    ///     true, it also enables showing an info card when the region is clicked.
    /// </remarks>
    public static void DrawThing(Rect region, ThingDef def, string? labelOverride = null, bool infoCard = true)
    {
        var iconRect = new Rect(region.x + 2f, region.y + 2f, region.height - 4f, region.height - 4f);
        var labelRect = new Rect(iconRect.x + region.height, region.y, region.width - region.height, region.height);

        Widgets.ThingIcon(iconRect, def);
        LabelDrawer.DrawLabel(labelRect, labelOverride ?? def.label?.CapitalizeFirst() ?? def.defName);

        if (Current.Game == null || !infoCard) return;

        if (Widgets.ButtonInvisible(region)) Find.WindowStack.Add(new Dialog_InfoCard(def));

        Widgets.DrawHighlightIfMouseover(region);
    }
}
