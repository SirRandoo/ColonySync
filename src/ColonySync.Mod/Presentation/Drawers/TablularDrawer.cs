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

using System;
using System.Collections.Generic;
using ColonySync.Mod.Presentation.Extensions;
using JetBrains.Annotations;
using UnityEngine;
using Verse;

namespace ColonySync.Mod.Presentation.Drawers;

/// <summary>A serialized class for drawing tabbed content on screen.</summary>
/// <remarks>
///     Developers wanting tabbed content should use the <see cref="TabularDrawer.Builder" />
///     class to instantiate a new tabular drawer.
/// </remarks>
[PublicAPI]
public class TabularDrawer
{
    /// <summary>A delegate used for drawing the content within a specified region.</summary>
    /// <param name="region">The region of the screen where the content is to be drawn.</param>
    public delegate void ContentDrawer(Rect region);

    private const int IconSize = 16;
    private const int IconTextPadding = 5;
    private static readonly Color BackgroundColor = new(r: 0.46f, g: 0.49f, b: 0.5f);
    private readonly Dictionary<Tab, Vector2> _labelCache = [];
    private int _currentPage = 1;
    private Tab? _currentTab;
    private float _foregroundProgress;
    private float _highlightProgress;
    private Tab? _lastTab;
    private float _maxHeight;
    private float _maxWidth;
    private Rect _previousRegion = Rect.zero;

    private IReadOnlyList<Tab> _tabs = [];
    private int _tabsPerView;
    private int _totalPages;

    private TabularDrawer()
    {
    }

    /// <summary>Provides the ID of the current active tab. Returns null if no tab is active.</summary>
    public string? CurrentTabId => _currentTab?.Id;

    /// <summary>Draws the tabular content on screen.</summary>
    /// <param name="region">The region of the screen to draw the tabular content in.</param>
    public void Draw(Rect region)
    {
        if (_previousRegion != region)
        {
            _previousRegion = region;
            RecalculateLayout();
        }

        var barRegion = new Rect(x: 0f, y: 0f, region.width, UiConstants.TabHeight);
        var contentRegion = new Rect(x: 0f, barRegion.height, region.width, region.height - barRegion.height);

        GUI.BeginGroup(region);

        GUI.color = BackgroundColor;
        Widgets.DrawLightHighlight(region.AtZero());
        GUI.color = Color.white;

        GUI.BeginGroup(barRegion);
        DrawTabs(barRegion);
        GUI.EndGroup();

        if (_currentTab != null) Widgets.DrawLightHighlight(contentRegion);

        contentRegion = contentRegion.ContractedBy(16f);

        GUI.BeginGroup(contentRegion);

        if (_currentTab == null)
        {
            GUI.EndGroup();

            return;
        }

        _currentTab.ContentDrawer?.Invoke(contentRegion.AtZero());
        GUI.EndGroup();

        GUI.EndGroup();
    }

    /// <summary>Draws the tabs in the given region of the screen.</summary>
    /// <param name="region">The region of the screen where the tabs will be drawn.</param>
    private void DrawTabs(Rect region)
    {
        var tabBarRegion = new Rect(x: 0f, y: 0f, region.width, region.height);

        if (_totalPages > 1)
        {
            var previousPageRegion = new Rect(x: 0f, y: 0f, region.height, region.height);
            var nextPageRegion = new Rect(region.width - region.height, y: 0f, region.height, region.height);

            DrawHorizontalNavigation(previousPageRegion, nextPageRegion);

            tabBarRegion.x = previousPageRegion.width;
            tabBarRegion.width = tabBarRegion.width - (previousPageRegion.width + nextPageRegion.width);
        }

        GUI.BeginGroup(tabBarRegion);

        var pageIndex = (_currentPage - 1) * _tabsPerView;
        var viewCount = Mathf.Min(_tabs.Count, _tabsPerView);
        float width = Mathf.FloorToInt(tabBarRegion.width / _tabsPerView);

        for (var i = 0; i < viewCount; i++)
        {
            var tabIndex = pageIndex + i;

            if (tabIndex >= _tabs.Count) break;

            var tab = _tabs[tabIndex];

            var tabRegion = new Rect(i * width, tabBarRegion.y, width, region.height);

            Widgets.DrawHighlightIfMouseover(tabRegion);
            TooltipHandler.TipRegion(tabRegion, tab.Tooltip);

            if (tab == _currentTab) DrawTabHighlight(tabRegion);

            if (Widgets.ButtonInvisible(tabRegion)) _currentTab = tab;

            GUI.BeginGroup(tabRegion.ContractedBy(5f));
            DrawTabContent(tabRegion.AtZero(), tab);
            GUI.EndGroup();
        }

        GUI.EndGroup();
    }

    /// <summary>Draws the horizontal navigation icons and handles page navigation logic.</summary>
    /// <param name="leftRegion">The region where the left navigation icon should be drawn.</param>
    /// <param name="rightRegion">The region where the right navigation icon should be drawn.</param>
    private void DrawHorizontalNavigation(Rect leftRegion, Rect rightRegion)
    {
        var leftRegionCenter = leftRegion.center;
        var rightRegionCenter = rightRegion.center;

        const float halvedIconSize = IconSize * 0.5f;
        var leftIconRegion = new Rect(leftRegionCenter.x - halvedIconSize, leftRegionCenter.y - halvedIconSize,
            IconSize, IconSize);
        var rightIconRegion = new Rect(rightRegionCenter.x - halvedIconSize, rightRegionCenter.y - halvedIconSize,
            IconSize, IconSize);

        IconDrawer.DrawIcon(leftIconRegion, _currentPage == 1 ? Icons.Get(IconCategory.Solid, "AnglesLeft") : Icons.Get(IconCategory.Solid, "AngleLeft"),
            Color.white);
        IconDrawer.DrawIcon(rightIconRegion,
            _currentPage == _totalPages ? Icons.Get(IconCategory.Solid, "AnglesRight") : Icons.Get(IconCategory.Solid, "AngleRight"), Color.white);

        if (Widgets.ButtonInvisible(leftRegion)) _currentPage = _currentPage <= 1 ? _totalPages : _currentPage - 1;

        if (Widgets.ButtonInvisible(rightRegion)) _currentPage = _currentPage >= _totalPages ? 1 : _currentPage + 1;

        Widgets.DrawHighlightIfMouseover(leftRegion);
        Widgets.DrawHighlightIfMouseover(rightRegion);
    }

    /// <summary>Draws the highlight for the active tab.</summary>
    /// <param name="region">The region of the screen where the highlight should be drawn.</param>
    private void DrawTabHighlight(Rect region)
    {
        if (_lastTab != _currentTab)
        {
            _highlightProgress = 0f;
            _foregroundProgress = 0f;
            _lastTab = _currentTab;
        }

        var cache = GUI.color;

        GUI.color = Color.cyan;
        var center = region.center;
        float accentDistance = Mathf.FloorToInt((region.x - center.x) * 0.75f);
        _highlightProgress = Mathf.SmoothStep(_highlightProgress, accentDistance, t: 0.15f);
        Widgets.DrawLineHorizontal(center.x - _highlightProgress, region.y + region.height - 1f,
            _highlightProgress * 2f);

        GUI.color = cache;

        _foregroundProgress = Mathf.SmoothStep(_foregroundProgress, center.x - region.x, t: 0.15f);
        var animationRegion = new Rect(center.x, center.y, width: 0f, height: 0f);

        Widgets.DrawLightHighlight(animationRegion.ExpandedBy(_foregroundProgress));
    }

    /// <summary>Draws the content of a tab within the specified region.</summary>
    /// <param name="region">The region of the screen where the tab content will be drawn.</param>
    /// <param name="tab">The tab whose content needs to be drawn.</param>
    private void DrawTabContent(Rect region, Tab tab)
    {
        Rect iconRegion;

        switch (tab.Layout)
        {
            case IconLayout.IconAndText:
                var center = region.center;
                var tabSize = _labelCache[tab];

                var contentWidth = tabSize.x - 10f;
                var halvedContentWidth = contentWidth * 0.5f;
                iconRegion = new Rect(center.x - halvedContentWidth, center.y - IconSize * 0.5f, IconSize, IconSize);

                var textRegion = new Rect(
                    iconRegion.x + IconSize + IconTextPadding,
                    center.y - UiConstants.LineHeight * 0.5f,
                    contentWidth - IconSize - IconTextPadding,
                    UiConstants.LineHeight
                );


                IconDrawer.DrawIcon(iconRegion, tab.Icon!, Color.white);
                LabelDrawer.DrawLabel(textRegion, tab.Label!);

                return;
            case IconLayout.Text:
                LabelDrawer.DrawLabel(region, tab.Label!, TextAnchor.MiddleCenter);

                return;
            case IconLayout.Icon:
                iconRegion = RectExtensions.IconRect(x: 0f, y: 0f, IconSize, region.height);
                IconDrawer.DrawIcon(iconRegion, tab.Icon!, Color.white);

                return;
            default:
                LabelDrawer.DrawLabel(region, tab.Label!, TextAnchor.MiddleCenter);

                return;
        }
    }

    /// <summary>Recalculates the layout and dimensions of the tabs based on the current region dimensions.</summary>
    private void RecalculateLayout()
    {
        var cache = Text.Font;
        Text.Font = GameFont.Small;

        for (var index = 0; index < _tabs.Count; index++)
        {
            var tab = _tabs[index];

            if (!_labelCache.TryGetValue(tab, out var vector))
            {
                var size = !string.IsNullOrEmpty(tab.Label) ? Text.CalcSize(tab.Label) : new Vector2();

                if (tab.Icon != null) size.IncrementX(IconSize + IconTextPadding);

                size.IncrementX(10f); // Padding

                _labelCache[tab] = vector = size;
            }

            if (vector.x > _maxWidth) _maxWidth = vector.x;

            if (vector.y > _maxHeight) _maxHeight = vector.y;
        }

        Text.Font = cache;

        var tabBarWidth = Mathf.CeilToInt(_previousRegion.width - UiConstants.TabHeight * 2f);
        _tabsPerView = Mathf.FloorToInt(tabBarWidth / _maxWidth);
        _totalPages = Mathf.CeilToInt(_tabs.Count / (float)_tabsPerView);
    }

    /// <summary>Represents a specialized class for building tabbed content.</summary>
    /// <remarks>
    ///     Use the Builder class to configure and construct instances of the
    ///     <see cref="TabularDrawer" />. This class provides methods for defining tabs with various
    ///     properties, allowing for customized tab layouts to be built and displayed on the screen.
    /// </remarks>
    [PublicAPI]
    public class Builder
    {
        /// <summary>Represents an action that configures an instance of <see cref="TabOptions" />.</summary>
        /// <param name="options">The tab options to configure.</param>
        public delegate void ConfigurationAction(TabOptions options);

        /// <summary>Holds the collection of all tabs added to the tabular drawer builder.</summary>
        private readonly List<Tab> _tabs = [];

        /// <summary>Adds a tab to the tabular drawer.</summary>
        /// <param name="id">The unique id of the tab.</param>
        /// <param name="configurator">
        ///     A callable that takes an instance of <see cref="TabOptions" /> that's
        ///     used to customize the tab being added. Each tab must have an icon and/or a label.
        /// </param>
        /// <returns>The builder itself.</returns>
        /// <exception cref="InvalidOperationException">
        ///     The tab's layout requires an icon or label be set
        ///     within the <see cref="TabOptions" /> instance.
        /// </exception>
        public Builder WithTab(string id, ConfigurationAction configurator)
        {
            var options = new TabOptions();

            configurator(options);

            var isLabelInvalid = string.IsNullOrEmpty(options.Label);

            if (isLabelInvalid && options.Icon == null)
                throw new InvalidOperationException("An icon or label must be when adding a new tab.");

            if (isLabelInvalid && options.Layout == IconLayout.Text)
                throw new InvalidOperationException("Tab layout requires a label to be specified.");

            if (options.Icon == null && options.Layout == IconLayout.Icon)
                throw new InvalidOperationException("Tab layout requires an icon to be specified.");

            _tabs.Add(new Tab(id, options.Label, options.Drawer, options.Tooltip, options.Icon, options.Layout));

            return this;
        }

        /// <summary>
        ///     Returns an instance of <see cref="TabularDrawer" /> prepped with each tab previously
        ///     specified.
        /// </summary>
        /// <returns>A new instance of <see cref="TabularDrawer" /> with configured tabs.</returns>
        public TabularDrawer Build()
        {
            var drawer = new TabularDrawer
            {
                _tabs = _tabs.AsReadOnly()
            };

            if (_tabs.Count > 0) drawer._currentTab = _tabs[0];

            return drawer;
        }
    }

    /// <summary>Represents the various options each tab can have.</summary>
    /// <remarks>
    ///     Each tab in the tabular drawer can be customized using the properties available in this
    ///     class. Tabs can have labels, tooltips, icons, and specific layouts. This class is sealed to
    ///     ensure it is only used as intended within the drawer's context.
    /// </remarks>
    [PublicAPI]
    public sealed class TabOptions
    {
        /// <summary>
        ///     Specifies the text label for a tab. If set to null or empty, certain layouts or
        ///     configurations may be invalid.
        /// </summary>
        public string? Label { get; set; }

        /// <summary>Gets or sets the tooltip text displayed when hovering over a tab.</summary>
        public string? Tooltip { get; set; }

        /// <summary>Provides a delegate function for rendering content within a specified screen region.</summary>
        public ContentDrawer? Drawer { get; set; }

        /// <summary>Gets or sets the icon for a tab.</summary>
        public Texture2D? Icon { get; set; }

        /// <summary>Specifies the layout type for the icon and text within a tab.</summary>
        public IconLayout Layout { get; set; } = IconLayout.Text;
    }

    /// <summary>Represents a tab within a tabular drawer.</summary>
    /// <remarks>
    ///     Tabs are used to encapsulate and manage separate sections of content within a tabular
    ///     interface. Each tab contains properties like an identifier, label, tooltip, and associated
    ///     content drawer.
    /// </remarks>
    private sealed record Tab(
        string Id,
        string? Label = null,
        ContentDrawer? ContentDrawer = null,
        string? Tooltip = null,
        Texture2D? Icon = null,
        IconLayout Layout = IconLayout.IconAndText
    );
}
