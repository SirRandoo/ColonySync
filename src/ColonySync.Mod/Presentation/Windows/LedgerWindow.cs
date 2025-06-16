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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ColonySync.Common.Interfaces;
using ColonySync.Common.Settings;
using ColonySync.Mod.Presentation.Dialogs;
using ColonySync.Mod.Presentation.Drawers;
using ColonySync.Mod.Presentation.Extensions;
using Microsoft.Extensions.Logging;
using RimWorld;
using UnityEngine;
using Verse;
using IIdentifiable = ColonySync.Common.Interfaces.IIdentifiable;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ColonySync.Mod.Presentation.Windows;

/// <summary>LedgerWindow class provides a UI window for displaying and interacting with ledger data.</summary>
public class LedgerWindow(ILogger logger, MoralitySettings moralitySettings) : Window
{
    private const float RowSplitPercentage = 0.35f;

    private readonly IReadOnlyList<ILedger>
        _dropdownItems = null!; // TODO: Ensure this points to all ledgers when ledger registry is implemented.

    private readonly QuickSearchWidget _searchWidget = new();

    private bool _isDebugInstance = false;
    private ILedger? _ledger;
    private Vector2 _listScrollPosition = Vector2.zero;
    private IUserData? _viewer;
    private string? _viewerBalanceBuffer;
    private bool _viewerBalanceBufferValid;
    private string? _viewerKarmaBuffer;
    private bool _viewerKarmaBufferValid;
    private IReadOnlyList<IUserData> _workingList = new List<IUserData>(0);

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

    // TODO: Replace the right branch with the actual viewer list.
    /// <summary>
    ///     Gets the list of viewers for the current ledger. When the instance is in debug mode, returns the ledger;
    ///     otherwise, returns null.
    /// </summary>
    protected ILedger? ViewerList => _isDebugInstance ? _ledger : null;

    /// <inheritdoc />
    public override void DoWindowContents(Rect inRect)
    {
        var ledgerListRegion = new Rect(x: 0f, y: 0f, inRect.width * 0.4f, inRect.height);

        var headerRegion = new Rect(
            ledgerListRegion.width + 10f, y: 0f, inRect.width - ledgerListRegion.width - 10f, UiConstants.LineHeight
        );

        var contentRegion = new Rect(
            headerRegion.x, headerRegion.height + 10f, headerRegion.width,
            ledgerListRegion.height - headerRegion.height - 10f
        );

        GUI.BeginGroup(inRect);

        GUI.BeginGroup(headerRegion);
        DrawHeader(headerRegion.AtZero());
        GUI.EndGroup();

        GUI.BeginGroup(ledgerListRegion);
        DrawLedgerOverview(ledgerListRegion.AtZero());
        GUI.EndGroup();

        GUI.BeginGroup(contentRegion);

        if (_viewer != null) DrawViewerPanel(contentRegion.AtZero());

        GUI.EndGroup();

        GUI.EndGroup();
    }

    private void DrawLedgerOverview(Rect region)
    {
        var searchRegion = new Rect(x: 0f, y: 0f, region.width, UiConstants.LineHeight);

        var listRegion = new Rect(
            x: 0f, UiConstants.LineHeight + 10f, region.width, region.height - UiConstants.LineHeight - 10f
        );

        var innerListRegion = listRegion.ContractedBy(5f);

        GUI.BeginGroup(searchRegion);
        _searchWidget.OnGUI(searchRegion.AtZero(), FilterViewerList);
        GUI.EndGroup();

        Widgets.DrawMenuSection(listRegion);

        GUI.BeginGroup(innerListRegion);
        DrawViewerList(innerListRegion.AtZero());
        GUI.EndGroup();
    }

    private void DrawViewerList(Rect region)
    {
        var totalViewers = _workingList.Count;
        var scrollViewHeight = UiConstants.LineHeight * totalViewers;

        var scrollView = new Rect(
            x: 0f, y: 0f, region.width - (scrollViewHeight > region.height ? 16f : 0f), scrollViewHeight
        );

        GUI.BeginGroup(region);
        _listScrollPosition = GUI.BeginScrollView(region, _listScrollPosition, scrollView);

        var workingListDirty = false;

        for (var index = 0; index < totalViewers; index++)
        {
            var lineRegion = new Rect(x: 0f, UiConstants.LineHeight * index, scrollView.width, UiConstants.LineHeight);

            if (!lineRegion.IsVisible(region, _listScrollPosition)) continue;

            if (index % 2 == 1) Widgets.DrawHighlight(lineRegion);

            var viewer = _workingList[index];

            if (_viewer == viewer) Widgets.DrawHighlightSelected(lineRegion);

            // TODO: Draw viewer's platform icon when platforms are implemented.
            LabelDrawer.DrawLabel(lineRegion, viewer.User.Name);

            if (ButtonDrawer.DrawFieldButton(lineRegion, TexButton.Delete, KitTranslations.DeleteViewerTooltip))
            {
                _ledger!.Data.Unregister(viewer);

                workingListDirty = true;
            }

            if (Widgets.ButtonInvisible(lineRegion))
            {
                _viewer = viewer;

                _viewerBalanceBuffer = viewer.Points.ToString();
                _viewerBalanceBufferValid = true;

                _viewerKarmaBuffer = viewer.Karma.ToString();
                _viewerKarmaBufferValid = true;

                Find.WindowStack.TryRemove(typeof(TransactionHistoryDialog), doCloseSound: false);
            }

            Widgets.DrawHighlightIfMouseover(lineRegion);
        }

        GUI.EndScrollView();
        GUI.EndGroup();

        if (workingListDirty) FilterViewerList();
    }

    private void DrawHeader(Rect region)
    {
        var (ledgerLabelRegion, ledgerDropdownRegion) = region.Split(RowSplitPercentage);

        LabelDrawer.DrawLabel(ledgerLabelRegion, KitTranslations.CurrentLedger, TextAnchor.MiddleRight);
        DropdownDrawer.Draw(ledgerDropdownRegion, _ledger!, _dropdownItems, UpdateLedger);
    }

    private void DrawViewerPanel(Rect region)
    {
        var lineRegion = new Rect(x: 0f, y: 0f, region.width, UiConstants.LineHeight);

        DrawUsernameField(lineRegion);

        lineRegion = lineRegion.Shift(Direction8Way.South);
        DrawBalanceField(lineRegion);

        lineRegion = lineRegion.Shift(Direction8Way.South);
        DrawKarmaField(lineRegion);


        lineRegion = lineRegion.Shift(Direction8Way.South);
        var (_, transactionsBtnRegion) = lineRegion.Split(RowSplitPercentage);

        if (Widgets.ButtonText(
                transactionsBtnRegion, KitTranslations.ViewTransactionsText, overrideTextAnchor: TextAnchor.MiddleCenter
            ))
        {
            // TODO:
        }

        var lastSeenRegion = new Rect(x: 0f, region.height - Text.SmallFontHeight, region.width, Text.SmallFontHeight);

        LabelDrawer.DrawLabel(
            lastSeenRegion, string.Format(KitTranslations.LedgerLastSeen, FormatLastSeen(_viewer!))
        );
    }

    private void DrawKarmaField(Rect region)
    {
        var (labelRegion, fieldRegion) = region.Split(RowSplitPercentage);

        LabelDrawer.DrawLabel(labelRegion, KitTranslations.CommonTextKarma);

        var addBtnRegion = new Rect(fieldRegion.x, fieldRegion.y, fieldRegion.height, fieldRegion.height);

        var rmvBtnRegion = new Rect(
            fieldRegion.x + fieldRegion.width - fieldRegion.height, fieldRegion.y, fieldRegion.height,
            fieldRegion.height
        );

        var numberFieldRegion = new Rect(
            fieldRegion.x + fieldRegion.height + 5f, fieldRegion.y, fieldRegion.width - fieldRegion.height * 2f - 10f,
            fieldRegion.height
        );

        if (FieldDrawer.DrawNumberField(
                numberFieldRegion, out int newKarma, ref _viewerKarmaBuffer!, ref _viewerKarmaBufferValid
            ))
            _viewer!.Karma = (short)newKarma;


        Text.Font = GameFont.Medium;

        if (Widgets.ButtonText(addBtnRegion, label: "+", overrideTextAnchor: TextAnchor.MiddleCenter))
        {
            var dialog = new ShortEntryDialog(
                maximum: (short)(moralitySettings.AlignmentRange.Minimum - _viewer!.Karma)
            );

            dialog.NumberEntered += (_, e) => _viewer.Karma += e;

            Find.WindowStack.Add(dialog);
        }

        TooltipHandler.TipRegion(addBtnRegion, KitTranslations.AddViewerKarmaTooltip);

        if (Widgets.ButtonText(rmvBtnRegion, label: "-", overrideTextAnchor: TextAnchor.MiddleCenter))
        {
            var dialog = new ShortEntryDialog(
                (short)(moralitySettings.AlignmentRange.Maximum + _viewer!.Karma), maximum: 0
            );

            dialog.NumberEntered += (_, e) => _viewer.Karma -= e;

            Find.WindowStack.Add(dialog);
        }

        TooltipHandler.TipRegion(rmvBtnRegion, KitTranslations.RemoveViewerKarmaTooltip);

        Text.Font = GameFont.Small;
    }

    private void DrawBalanceField(Rect region)
    {
        var (labelRegion, fieldRegion) = region.Split(RowSplitPercentage);

        LabelDrawer.DrawLabel(labelRegion, KitTranslations.CommonTextBalance);


        var addBtnRegion = new Rect(fieldRegion.x, fieldRegion.y, fieldRegion.height, fieldRegion.height);

        var rmvBtnRegion = new Rect(
            fieldRegion.x + fieldRegion.width - fieldRegion.height, fieldRegion.y, fieldRegion.height,
            fieldRegion.height
        );

        var numberFieldRegion = new Rect(
            fieldRegion.x + fieldRegion.height + 5f, fieldRegion.y, fieldRegion.width - fieldRegion.height * 2f - 10f,
            fieldRegion.height
        );

        if (FieldDrawer.DrawNumberField(
                numberFieldRegion, out int newBalance, ref _viewerBalanceBuffer!,
                ref _viewerBalanceBufferValid
            ))
            _viewer!.Points = newBalance;


        Text.Font = GameFont.Medium;

        if (Widgets.ButtonText(addBtnRegion, label: "+", overrideTextAnchor: TextAnchor.MiddleCenter))
        {
            var dialog = new LongEntryDialog(maximum: int.MaxValue - _viewer!.Points);
            dialog.NumberEntered += (_, e) => _viewer.Points += e;

            Find.WindowStack.Add(dialog);
        }

        TooltipHandler.TipRegion(addBtnRegion, KitTranslations.AddViewerPointsTooltip);

        if (Widgets.ButtonText(rmvBtnRegion, label: "-", overrideTextAnchor: TextAnchor.MiddleCenter))
        {
            var dialog = new LongEntryDialog(int.MinValue + _viewer!.Points, maximum: 0);
            dialog.NumberEntered += (_, e) => _viewer.Points -= e;

            Find.WindowStack.Add(dialog);
        }

        TooltipHandler.TipRegion(rmvBtnRegion, KitTranslations.RemoveViewerPointsTooltip);

        Text.Font = GameFont.Small;
    }

    private void DrawUsernameField(Rect region)
    {
        var (nameLabelRegion, nameFieldRegion) = region.Split(RowSplitPercentage);

        LabelDrawer.DrawLabel(nameLabelRegion, KitTranslations.CommonTextUsername);
        LabelDrawer.DrawLabel(nameFieldRegion, _viewer!.User.Name);
    }

    private void FilterViewerList()
    {
        if (string.IsNullOrEmpty(_searchWidget.filter.Text))
            _workingList = ViewerList!.Data.AllRegistrants;

        var copy = new List<IUserData>();
        var registrants = ViewerList!.Data.AllRegistrants;


        for (var index = 0; index < registrants.Count; index++)
        {
            var viewer = registrants[index];

            if (string.Compare(viewer.Name, _searchWidget.filter.Text, StringComparison.InvariantCultureIgnoreCase)
                > 0)
                copy.Add(viewer);
            else if (viewer == _viewer) _viewer = null;
        }

        _workingList = new ReadOnlyCollection<IUserData>(copy);
    }

    private void UpdateLedger(IIdentifiable newLedger)
    {
        _ledger = (ILedger)newLedger;

        FilterViewerList();
    }

    private static string FormatLastSeen(IUserData viewer)
    {
        var currentTime = DateTime.UtcNow;
        var offset = currentTime - viewer.User.LastSeen;

        var totalDays = offset.TotalDays;

        if (totalDays > 0) return string.Format(KitTranslations.LedgerLastSeenDays, totalDays.ToString("F2"));

        var totalHours = offset.TotalHours;

        if (totalHours > 0) return string.Format(KitTranslations.LedgerLastSeenHours, totalHours.ToString("F2"));

        var totalMinutes = offset.TotalMinutes;

        if (totalMinutes > 0) return string.Format(KitTranslations.LedgerLastSeenMinutes, totalMinutes.ToString("F2"));

        return string.Format(KitTranslations.LedgerLastSeenSeconds, offset.TotalSeconds.ToString("F2"));
    }

    /// <inheritdoc />
    public override void PreOpen()
    {
        base.PreOpen();

        if (ViewerList is not null)
        {
            _workingList = ViewerList.Data.AllRegistrants;

            return;
        }

        logger.LogWarning(
            "Closing window as no ledger was provided to the ledger window. If this happens repeatedly, you should ask for help from the developer(s)"
        );

        Close(false);
    }
}
