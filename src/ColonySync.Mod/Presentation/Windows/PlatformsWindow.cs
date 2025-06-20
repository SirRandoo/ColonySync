// MIT License
// 
// Copyright (c) 2024 sirrandoo
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

using System;
using System.Collections.Generic;
using ColonySync.Common.Interfaces;
using ColonySync.Mod.Presentation.Drawers;
using ColonySync.Mod.Presentation.Extensions;
using Microsoft.Extensions.Logging;
using RimWorld;
using UnityEngine;
using Verse;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ColonySync.Mod.Presentation.Windows;

public sealed class PlatformsWindow(ILogger logger) : Window
{
    private const float RowSplitPercentage = 0.35f;
    private readonly Dictionary<string, Texture2D> _platformIcons = [];
    private readonly IReadOnlyList<IPlatform> _platforms = null!;
    private readonly QuickSearchWidget _searchWidget = new();

    private bool _isDebugInstance = false;
    private Vector2 _listScrollPosition = Vector2.zero;
    private IPlatform? _platform;

    private IReadOnlyList<IPlatform> _workingList = new List<IPlatform>();

    // TODO: Replace the right branch with the actual platform.
    private IPlatform? Platform => _isDebugInstance ? _platform : null;

    /// <inheritdoc />
    public override Vector2 InitialSize
    {
        get
        {
            var initialSize = base.InitialSize;
            initialSize.IncrementX(100);

            return initialSize;
        }
    }

    /// <inheritdoc />
    public override void DoWindowContents(Rect inRect)
    {
        var platformListRegion = new Rect(x: 0f, y: 0f, inRect.width * 0.4f, inRect.height);

        var contentRegion = new Rect(
            platformListRegion.width + 10f, y: 0f, inRect.width - platformListRegion.width - 10f, inRect.height
        );

        GUI.BeginGroup(inRect);

        GUI.BeginGroup(platformListRegion);
        DrawPlatformOverview(platformListRegion.AtZero());
        GUI.EndGroup();

        GUI.BeginGroup(contentRegion);

        if (_platform != null) DrawPlatformPanel(contentRegion.AtZero());

        GUI.EndGroup();

        GUI.EndGroup();
    }

    private void DrawPlatformOverview(Rect region)
    {
        var searchRegion = new Rect(x: 0f, y: 0f, region.width, UiConstants.LineHeight);

        var listRegion = new Rect(
            x: 0f, UiConstants.LineHeight + 10f, region.width, region.height - UiConstants.LineHeight - 10f
        );

        var innerListRegion = listRegion.ContractedBy(5f);

        GUI.BeginGroup(searchRegion);
        _searchWidget.OnGUI(searchRegion.AtZero(), FilterPlatformList);
        GUI.EndGroup();

        Widgets.DrawMenuSection(listRegion);

        GUI.BeginGroup(innerListRegion);
        DrawPlatformList(innerListRegion.AtZero());
        GUI.EndGroup();
    }

    private void DrawPlatformList(Rect region)
    {
        var totalPlatforms = _workingList.Count;
        var scrollViewHeight = UiConstants.LineHeight * totalPlatforms;

        var scrollView = new Rect(
            x: 0f, y: 0f, region.width - (scrollViewHeight > region.height ? 16f : 0f), scrollViewHeight
        );

        GUI.BeginGroup(region);
        _listScrollPosition = GUI.BeginScrollView(region, _listScrollPosition, scrollView);

        for (var index = 0; index < totalPlatforms; index++)
        {
            var lineRegion = new Rect(x: 0f, UiConstants.LineHeight * index, scrollView.width, UiConstants.LineHeight);

            if (!lineRegion.IsVisible(region, _listScrollPosition)) continue;

            if (index % 2 == 1) Widgets.DrawHighlight(lineRegion);

            var platform = _workingList[index];

            if (_platform == platform) Widgets.DrawHighlightSelected(lineRegion);

            LabelDrawer.DrawLabel(lineRegion, platform.Name);

            if (_platformIcons.TryGetValue(platform.Id, out var platformIcon))
                IconDrawer.DrawFieldIcon(lineRegion, platformIcon);

            if (Widgets.ButtonInvisible(lineRegion)) _platform = platform;

            Widgets.DrawHighlightIfMouseover(lineRegion);
        }

        GUI.EndScrollView();
        GUI.EndGroup();
    }

    private void FilterPlatformList()
    {
        if (string.IsNullOrEmpty(_searchWidget.filter.Text))
            _workingList = _platforms; // TODO: Replace this with the actual platform list.

        var copy = new List<IPlatform>();
        var registrants = _platforms;

        for (var index = 0; index < registrants.Count; index++)
        {
            var platform = registrants[index];

            if (string.Compare(platform.Id, _searchWidget.filter.Text, StringComparison.InvariantCultureIgnoreCase)
                > 0)
                copy.Add(platform);
            else if (platform == _platform) _platform = null;
        }

        _workingList = copy;
    }

    private void DrawPlatformPanel(Rect region)
    {
    }

    /// <inheritdoc />
    public override void PreOpen()
    {
        base.PreOpen();

        if (Platform is not null)
        {
            _workingList = _platforms;

            return;
        }

        logger.LogWarning(
            "Closing window as no platforms are registered. If this happens repeatedly, you should ask for help from the developer(s)."
        );

        Close(false);
    }
}
