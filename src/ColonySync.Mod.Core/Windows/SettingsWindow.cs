// MIT License
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

using ColonySync.Mod.Core.Pages;
using ColonySync.Mod.Presentation;
using ColonySync.Mod.Presentation.Drawers;
using UnityEngine;
using Verse;

namespace ColonySync.Mod.Core.Windows;

// TODO: Currently point decay settings aren't drawn on screen.

/// <summary>
///     Represents a settings window in the application. Inherits from the ProxySettingsWindow class and provides
///     multiple tabs to display various settings for the application.
/// </summary>
internal sealed partial class SettingsWindow : ProxySettingsWindow
{
    private readonly ClientSettingsPage _clientSettingsPage = new();
    private readonly CommandSettingsPage _commandSettingsPage = new();
    private readonly MoralitySettingsPage _moralitySettingsPage = new();
    private readonly PawnSettingsPage _pawnSettingsPage = new();
    private readonly PointSettingsPage _pointSettingsPage = PointSettingsPage.CreateInstance();
    private readonly PollSettingsPage _pollSettingsPage = new();
    private readonly StoreSettingsPage _storeSettingsPage = new();
    private readonly TabularDrawer _tabWorker;

    /// <inheritdoc />
    public SettingsWindow(Verse.Mod mod) : base(mod)
    {
        doCloseX = false;

        _tabWorker = new TabularDrawer.Builder()
            .WithTab(
                id: "client",
                o =>
                {
                    o.Icon = Icons.Get(IconCategory.Solid, "Person");
                    o.Layout = IconLayout.IconAndText;
                    o.Drawer = _clientSettingsPage.Draw;
                    o.Label = KitTranslations.SettingsTabClient;
                    o.Tooltip = KitTranslations.SettingsTabClientTooltip;
                }
            )
            .WithTab(
                id: "commands",
                options =>
                {
                    options.Icon = Icons.Get(IconCategory.Solid, "Message");
                    options.Layout = IconLayout.IconAndText;
                    options.Drawer = _commandSettingsPage.Draw;
                    options.Label = KitTranslations.SettingsTabCommands;

                    options.Tooltip = KitTranslations
                        .SettingsTabCommandTooltip;
                }
            )
            .WithTab(
                id: "morality",
                options =>
                {
                    options.Icon = Icons.Get(IconCategory.Solid, "ScaleBalanced");
                    options.Layout = IconLayout.IconAndText;
                    options.Drawer = _moralitySettingsPage.Draw;
                    options.Label = KitTranslations.SettingsTabMorality;

                    options.Tooltip = KitTranslations
                        .SettingsTabMoralityTooltip;
                }
            )
            .WithTab(
                id: "pawns",
                options =>
                {
                    options.Icon = Icons.Get(IconCategory.Solid, "PeopleGroup");
                    options.Layout = IconLayout.IconAndText;
                    options.Drawer = _pawnSettingsPage.Draw;
                    options.Label = KitTranslations.SettingsTabPawns;
                    options.Tooltip = KitTranslations.SettingsTabPawnsTooltip;
                }
            )
            .WithTab(
                id: "points",
                options =>
                {
                    options.Icon = Icons.Get(IconCategory.Solid, "PiggyBank");
                    options.Layout = IconLayout.IconAndText;
                    options.Drawer = _pointSettingsPage.Draw;
                    options.Label = KitTranslations.SettingsTabPoints;

                    options.Tooltip = KitTranslations
                        .SettingsTabPointsTooltip;
                }
            )
            .WithTab(
                id: "poll",
                options =>
                {
                    options.Icon = Icons.Get(IconCategory.Solid, "SquarePollVertical");
                    options.Layout = IconLayout.IconAndText;
                    options.Drawer = _pollSettingsPage.Draw;
                    options.Label = KitTranslations.SettingsTabPoll;
                    options.Tooltip = KitTranslations.SettingsTabPollTooltip;
                }
            )
            .WithTab(
                id: "store",
                options =>
                {
                    options.Icon = Icons.Get(IconCategory.Solid, "Store");
                    options.Layout = IconLayout.IconAndText;
                    options.Drawer = _storeSettingsPage.Draw;
                    options.Label = KitTranslations.SettingsTabStore;
                    options.Tooltip = KitTranslations.SettingsTabStoreTooltip;
                }
            )
#if DEBUG
            .WithTab(
                id: "debug",
                o =>
                {
                    o.Drawer = DrawDebugTab;
                    o.Icon = Icons.Get(IconCategory.Solid, "Bug");
                    o.Layout = IconLayout.IconAndText;
                    o.Label = KitTranslations.SettingsTabDebug;
                    o.Tooltip = KitTranslations.SettingsTabDebugTooltip;
                }
            )
#endif
            .Build();
    }

    /// <inheritdoc />
    protected override float Margin => 0f;

    /// <inheritdoc />
    public override void DoWindowContents(Rect inRect)
    {
        GUI.BeginGroup(inRect);

        _tabWorker.Draw(inRect.AtZero());

        GUI.EndGroup();
    }
}
