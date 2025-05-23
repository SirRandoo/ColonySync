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
using Steamworks;
using UnityEngine;
using Verse;

namespace ColonySync.Mod.Presentation.Drawers;

/// <summary>Provides drawing functionality for group headers in the user interface.</summary>
[PublicAPI]
public static class GroupDrawer
{
    /// <summary>Draws a group header within the listing.</summary>
    /// <param name="listing">The listing in which to draw the group header.</param>
    /// <param name="name">The name to display as the header.</param>
    /// <param name="gap">If true, adds a gap before drawing the group header; otherwise, no gap is added.</param>
    public static void DrawGroupHeader(this Listing listing, string name, bool gap = true)
    {
        if (gap) listing.Gap(UiConstants.LineHeight);

        LabelDrawer.DrawLabel(listing.GetRect(Text.LineHeight), name, TextAnchor.LowerLeft, GameFont.Tiny);
        listing.GapLine(6f);
    }

    /// <summary>Draws a header for a mod group with the specified name and mod ID.</summary>
    /// <param name="listing">The listing to draw the header on.</param>
    /// <param name="modName">The name of the mod group to display.</param>
    /// <param name="modId">The Steam Workshop ID of the mod.</param>
    /// <param name="gap">Specifies if a gap should be added before the header.</param>
    public static void DrawModGroupHeader(this Listing listing, string modName, ulong modId, bool gap = true)
    {
        if (gap) listing.Gap(UiConstants.LineHeight);

        var lineRect = listing.GetRect(Text.LineHeight);
        LabelDrawer.DrawLabel(lineRect, modName, TextAnchor.LowerLeft, GameFont.Tiny);

        var modRequirementString = string.Format(UxLocale.ModDependsOn, modName);

        Text.Font = GameFont.Tiny;
        var width = Text.CalcSize(modRequirementString).x;
        var modRequirementRect = new Rect(lineRect.x + lineRect.width - width, lineRect.y, width, Text.LineHeight);
        Text.Font = GameFont.Small;

        LabelDrawer.DrawLabel(lineRect, modRequirementString, UxColors.RedishPink, TextAnchor.LowerRight,
            GameFont.Tiny);

        Widgets.DrawHighlightIfMouseover(modRequirementRect);

        if (Widgets.ButtonInvisible(modRequirementRect)) SteamUtility.OpenWorkshopPage(new PublishedFileId_t(modId));

        listing.GapLine(6f);
    }
}