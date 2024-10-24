﻿// MIT License
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
using StreamKit.UX.Extensions;
using UnityEngine;
using Verse;

namespace StreamKit.UX.Drawers;

[PublicAPI]
public static class IconDrawer
{
    /// <summary>
    ///     Draws the specified texture in the color given.
    /// </summary>
    /// <param name="region">The region to draw the texture in</param>
    /// <param name="icon">The texture to draw</param>
    /// <param name="color">The color to draw the texture</param>
    /// <remarks>
    ///     This method doesn't recolor the texture given to the color specified; it changes the value of
    ///     <see cref="GUI.color" /> to the color specified and lets Unity handle the recoloring.
    /// </remarks>
    public static void DrawIcon(Rect region, Texture2D icon, Color? color)
    {
        region = RectExtensions.IconRect(region.x, region.y, region.width, region.height);

        Color old = GUI.color;

        GUI.color = color ?? Color.white;
        GUI.DrawTexture(region, icon);
        GUI.color = old;
    }

    /// <summary>
    ///     An internal method for creating a <see cref="Rect" /> suitable for
    ///     drawing "field icons" in.
    /// </summary>
    /// <param name="parentRegion">
    ///     The region of the field the icon is being
    ///     drawn over
    /// </param>
    /// <param name="offset">
    ///     An optional number indicating how many slots to
    ///     offset the icon
    /// </param>
    /// <returns>The <see cref="Rect" /> to draw the field icon in</returns>
    internal static Rect GetFieldIconRect(Rect parentRegion, int offset = 0) => RectExtensions.IconRect(
        parentRegion.x + parentRegion.width - parentRegion.height * (offset + 1),
        parentRegion.y,
        parentRegion.height,
        parentRegion.height,
        Mathf.CeilToInt(parentRegion.height * 0.1f)
    );

    /// <summary>
    ///     Draws an icon over an input field.
    /// </summary>
    /// <param name="parentRegion">
    ///     The region of the field the icon is being
    ///     drawn over
    /// </param>
    /// <param name="icon">A character to be used as the icon</param>
    /// <param name="tooltip">An optional tooltip for the icon</param>
    /// <param name="offset">
    ///     An optional number indicated how many slots to
    ///     offset the icon
    /// </param>
    public static void DrawFieldIcon(Rect parentRegion, char icon, string? tooltip = null, int offset = 0)
    {
        Rect region = GetFieldIconRect(parentRegion, offset);
        LabelDrawer.DrawLabel(region, icon.ToString(), TextAnchor.MiddleCenter);
        TooltipHandler.TipRegion(region, tooltip);
    }

    /// <summary>
    ///     Draws an icon over an input field.
    /// </summary>
    /// <param name="parentRegion">
    ///     The region of the field the icon is being
    ///     drawn over
    /// </param>
    /// <param name="icon">A string to be used as the icon</param>
    /// <param name="tooltip">An optional tooltip for the icon</param>
    /// <param name="offset">
    ///     An optional number indicated how many slots to
    ///     offset the icon
    /// </param>
    public static void DrawFieldIcon(Rect parentRegion, string icon, string? tooltip = null, int offset = 0)
    {
        Rect region = GetFieldIconRect(parentRegion, offset);
        LabelDrawer.DrawLabel(region, icon, TextAnchor.MiddleCenter);
        TooltipHandler.TipRegion(region, tooltip);
    }

    /// <summary>
    ///     Draws an icon over an input field.
    /// </summary>
    /// <param name="parentRegion">
    ///     The region of the field the button is
    ///     being drawn over
    /// </param>
    /// <param name="icon">A texture to be drawn as the icon</param>
    /// <param name="tooltip">An optional tooltip for the icon</param>
    /// <param name="offset">
    ///     An optional number indicated how many slots to
    ///     offset the icon
    /// </param>
    public static void DrawFieldIcon(Rect parentRegion, Texture2D icon, string? tooltip = null, int offset = 0)
    {
        Rect region = GetFieldIconRect(parentRegion, offset);
        GUI.DrawTexture(region, icon);
        TooltipHandler.TipRegion(region, tooltip);
    }

    public static void DrawExperimentalIconCutout(ref Rect region)
    {
        Rect cutout = RectExtensions.IconRect(region.x, region.y, region.height, region.height, 6f);

        region.x = region.x + region.height;
        region.width = region.width - region.height;

        DrawIcon(cutout, Icons.TriangleExclamation.Value, UxColors.RedishPink);

        if (Mouse.IsOver(cutout))
        {
            TooltipHandler.TipRegion(cutout, UxLocale.ExperimentalNoticeColored);
        }
    }

    public static void DrawSortIndicator(Rect parentRegion, SortOrder order)
    {
        Rect region = RectExtensions.IconRect(
            parentRegion.x + parentRegion.width - parentRegion.height + 3f,
            parentRegion.y + 8f,
            parentRegion.height - 9f,
            parentRegion.height - 16f,
            0f
        );

        switch (order)
        {
            case SortOrder.Ascending:
                GUI.DrawTexture(region, Icons.SortUp.Value);

                return;
            case SortOrder.Descending:
                GUI.DrawTexture(region, Icons.SortDown.Value);

                return;
            case SortOrder.None:
                GUI.DrawTexture(region, Icons.Sort.Value);

                return;
        }
    }

    public static void DrawThing(Rect region, ThingDef def, string? labelOverride = null, bool infoCard = true)
    {
        var iconRect = new Rect(region.x + 2f, region.y + 2f, region.height - 4f, region.height - 4f);
        var labelRect = new Rect(iconRect.x + region.height, region.y, region.width - region.height, region.height);

        Widgets.ThingIcon(iconRect, def);
        LabelDrawer.DrawLabel(labelRect, labelOverride ?? def.label?.CapitalizeFirst() ?? def.defName);

        if (Current.Game == null || !infoCard)
        {
            return;
        }

        if (Widgets.ButtonInvisible(region))
        {
            Find.WindowStack.Add(new Dialog_InfoCard(def));
        }

        Widgets.DrawHighlightIfMouseover(region);
    }
}
