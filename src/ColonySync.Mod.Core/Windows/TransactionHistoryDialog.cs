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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using ColonySync.Common.Interfaces;
using ColonySync.Mod.Core.UX;
using ColonySync.Mod.Presentation;
using ColonySync.Mod.Presentation.Drawers;
using ColonySync.Mod.Presentation.Extensions;
using RimWorld;
using UnityEngine;
using Verse;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ColonySync.Mod.Core.Windows;

// TODO: The transaction history dialog currently doesn't show column names.
// TODO: Refunded transactions may require explicit support within the ITransaction interface to provide better information to streamers.
// TODO: The transaction history dialog currently doesn't have icons (outside of the search icon).
// TODO: The transaction history dialog currently doesn't have a way to refund transactions.
// TODO: The transaction history dialog currently doesn't have a way to individually remove transactions, or in bulk.
// TODO: The transaction history dialog currently doesn't have a way to mark transactions as "excluded" from the reputation system.
// TODO: Transactions currently use an enum to describe purchased goods. This will limit 3rd party developer's use of the transaction system.

/// <summary>A dialog for showing the transaction history of a viewer.</summary>
public sealed class TransactionHistoryDialog : Window
{
    private readonly ILogger _logger;
    private readonly QuickSearchWidget _searchWidget = new();
    private readonly TransactionTableDrawer _tableDrawer = null!;
    private readonly IUserData _viewer;
    private readonly IReadOnlyList<ITransaction> _workingList;
    private bool _isSearchExpanded;
    private Vector2 _scrollPosition = Vector2.zero;
    private float _searchOpenTime;

    private TransactionHistoryDialog(IUserData viewer, ILogger logger)
    {
        _logger = logger;
        _viewer = viewer;
        _workingList = new ReadOnlyCollection<ITransaction>(viewer.Transactions);

        layer = WindowLayer.Dialog;
    }

    /// <inheritdoc />
    public override Vector2 InitialSize => new(x: 600, y: 500);

    /// <inheritdoc />
    public override void DoWindowContents(Rect inRect)
    {
        var headerRegion = new Rect(x: 0f, y: 0f, inRect.width, UiConstants.LineHeight);

        var contentRegion = new Rect(
            x: 0f, UiConstants.LineHeight + 5f, inRect.width, inRect.height - UiConstants.LineHeight - 5f
        );

        GUI.BeginGroup(inRect);

        GUI.BeginGroup(headerRegion);
        DrawHeader(headerRegion);
        GUI.EndGroup();

        GUI.BeginGroup(contentRegion);
        _tableDrawer.Draw(contentRegion.AtZero());
        GUI.EndGroup();

        GUI.EndGroup();
    }

    private void DrawHeader(Rect region)
    {
        var titleRegion = new Rect(x: 0f, y: 0f, Mathf.FloorToInt(region.width * 0.5f), region.height);

        var searchRegion = new Rect(
            titleRegion.width + 5f, y: 0f, region.width - titleRegion.width - 5f, region.height
        );

        var searchBtnRegion = RectExtensions.IconRect(
            region.width - region.height, y: 0f, region.height, region.height
        );

        // TODO: Include the platform icon with the header text.
        LabelDrawer.DrawLabel(
            titleRegion, string.Format(KitTranslations.TransactionPurchaseHistoryDialogText, _viewer.User.Name)
        );

        if (!_isSearchExpanded)
        {
            GUI.DrawTexture(searchBtnRegion, TexButton.Search);

            if (Widgets.ButtonInvisible(searchBtnRegion))
            {
                _isSearchExpanded = true;
                _searchOpenTime = Time.unscaledTime;
            }
        }

        if (_isSearchExpanded) _searchWidget.OnGUI(searchRegion, FilterTransactions);
    }

    private void FilterTransactions()
    {
        _tableDrawer.NotifySearchRequested(_searchWidget.filter.Text);
    }

    /// <inheritdoc />
    public override void WindowUpdate()
    {
        base.WindowUpdate();

        TryCollapseSearchWidget();
    }

    private void TryCollapseSearchWidget()
    {
        if (!string.IsNullOrEmpty(_searchWidget.filter.Text)) return;

        if (_searchOpenTime <= 0) return;

        if (Time.unscaledTime - _searchOpenTime >= 10)
        {
            _searchOpenTime = 0f;
            _isSearchExpanded = false;
        }
    }
}