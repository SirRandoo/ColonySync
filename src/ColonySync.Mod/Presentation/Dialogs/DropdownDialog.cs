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
using UnityEngine;
using Verse;

namespace ColonySync.Mod.Presentation.Dialogs;

// TODO: Currently "DropdownDialog" is a modified version of DropdownDrawer's DropdownDialog internal class.

/// <summary>Represents a dropdown dialog window that allows selection from a list of options.</summary>
/// <typeparam name="T">The type of the options displayed in the dropdown.</typeparam>
public abstract class DropdownDialog<T> : Window
{
    /// The maximum number of options that can be displayed in the dropdown menu at once.
    private const int TotalDisplayableOptions = 9;

    /// <summary>Action delegate used to set the selected option in the dropdown.</summary>
    private readonly Action<T> _setter;

    /// <summary>Stores the total number of options available in the dropdown dialog.</summary>
    private readonly int _totalOptions;

    /// The currently selected option displayed in the dropdown dialog.
    protected readonly T CurrentOption;

    /// <summary>Represents the rectangular region on the screen where the dropdown list is displayed.</summary>
    protected readonly Rect DropdownRegion;

    /// <summary>Stores the current scroll position in a dropdown dialog scroll view.</summary>
    private Vector2 _scrollPos = new(x: 0f, y: 0f);

    /// Indicates whether the dropdown dialog is displayed in a reversed orientation.
    protected bool IsReversed;

    /// The height of the viewable area for the dropdown options, dynamically calculated based on the
    /// number of available options and constrained by a predefined maximum number of displayable options.
    protected float ViewHeight;

    /// Represents a dropdown dialog window in UI.
    protected DropdownDialog(Rect parentRegion, T current, IReadOnlyList<T> allOptions, Action<T> setter)
    {
        _setter = setter;
        Options = allOptions;
        CurrentOption = current;
        DropdownRegion = parentRegion;
        _totalOptions = allOptions.Count;

        doCloseX = false;
        drawShadow = false;
        doCloseButton = false;
        layer = WindowLayer.Dialog;

        closeOnClickedOutside = true;
        absorbInputAroundWindow = true;
        forceCatchAcceptAndCancelEventEvenIfUnfocused = true;
    }

    /// Gets the margin size for the dropdown dialog window.
    protected override float Margin => 5f;

    /// A list of options available in the dropdown dialog.
    /// This property is populated during the initialization of the dialog
    /// and is used to display the available options to the user.
    public IReadOnlyList<T> Options { get; set; }

    /// <inheritdoc />
    protected override void SetInitialSizeAndPosition()
    {
        var yPosition =
            IsReversed ? DropdownRegion.y - ViewHeight - 10f : DropdownRegion.y + DropdownRegion.height + 5f;

        windowRect = new Rect(DropdownRegion.x, yPosition, DropdownRegion.width, ViewHeight);
        windowRect = windowRect.ExpandedBy(5f);
    }

    /// Draws the contents of the dropdown window.
    /// <param name="inRect">The rectangle area where the contents will be drawn.</param>
    public override void DoWindowContents(Rect inRect)
    {
        var dropdownOptionsRegion = new Rect(x: 0f, y: 0f, inRect.width, inRect.height);

        GUI.BeginGroup(inRect);

        Widgets.DrawLightHighlight(dropdownOptionsRegion);
        Widgets.DrawLightHighlight(dropdownOptionsRegion);
        Widgets.DrawLightHighlight(dropdownOptionsRegion);

        GUI.BeginGroup(dropdownOptionsRegion);
        DrawDropdownOptions(dropdownOptionsRegion.AtZero());
        GUI.EndGroup();

        GUI.EndGroup();
    }

    /// <summary>Draws the options of the dropdown menu within the specified region.</summary>
    /// <param name="region">The rectangular area within which the dropdown options should be drawn.</param>
    protected void DrawDropdownOptions(Rect region)
    {
        var viewportWidth = region.width - (_totalOptions > TotalDisplayableOptions ? 16f : 0f);
        var viewport = new Rect(x: 0f, y: 0f, viewportWidth, UiConstants.LineHeight * _totalOptions);

        GUI.BeginGroup(region);
        _scrollPos = GUI.BeginScrollView(region.AtZero(), _scrollPos, viewport);

        for (var i = 0; i < _totalOptions; i++)
        {
            var lineRegion = new Rect(x: 0f, UiConstants.LineHeight * i, viewportWidth, UiConstants.LineHeight);

            if (!lineRegion.IsVisible(viewport, _scrollPos)) continue;

            var item = Options[i];
            var textRegion = new Rect(x: 5f, lineRegion.y, lineRegion.width - 10f, lineRegion.height);

            DrawItemLabel(textRegion, item);

            Widgets.DrawHighlightIfMouseover(lineRegion);

            if (AreItemsEqual(item, CurrentOption)) Widgets.DrawHighlightSelected(lineRegion);

            if (i % 2 == 0)
            {
                GUI.color = new Color(r: 0.5f, g: 0.5f, b: 0.5f, a: 0.44f);

                Widgets.DrawLineHorizontal(lineRegion.x, lineRegion.y, lineRegion.width);
                Widgets.DrawLineHorizontal(lineRegion.x, lineRegion.y + lineRegion.height, lineRegion.width);

                GUI.color = Color.white;
            }

            if (Widgets.ButtonInvisible(lineRegion, doMouseoverSound: false))
            {
                _setter(item);

                Close();
            }
        }

        GUI.EndScrollView();
        GUI.EndGroup();
    }

    /// <inheritdoc />
    public override void PreOpen()
    {
        ViewHeight = UiConstants.LineHeight *
                     (_totalOptions > TotalDisplayableOptions ? TotalDisplayableOptions : _totalOptions);
        IsReversed = DropdownRegion.y + ViewHeight > UI.screenHeight;

        base.PreOpen();
    }

    /// Handles the actions to be performed after the dropdown dialog is opened.
    /// This includes verifying that there are options to display and, if a current option is set,
    /// scrolling to that option in the dropdown list.
    public override void PostOpen()
    {
        if (Options.Count <= 0)
        {
            Log.Error("[SirRandoo.UX] Attempted to create a dropdown menu with no options.");

            Close(false);

            return;
        }

        if (CurrentOption != null) ScrollToItem(CurrentOption);
    }

    /// Scrolls the dropdown view to the specified item, centering it within the visible area if possible.
    /// <param name="item">The item to scroll to within the dropdown list.</param>
    private void ScrollToItem(T item)
    {
        var totalViewHeight = _totalOptions * UiConstants.LineHeight;

        for (var i = 0; i < _totalOptions; i++)
        {
            if (!AreItemsEqual(Options[i], item)) continue;

            var startingPageItem = i - Mathf.FloorToInt(TotalDisplayableOptions / 2f) - 1;

            _scrollPos = new Vector2(x: 0f, Mathf.Clamp(startingPageItem * UiConstants.LineHeight, min: 0f, totalViewHeight));
        }
    }

    /// <summary>Retrieves the label for a given item.</summary>
    /// <param name="item">The item for which the label is to be retrieved.</param>
    /// <returns>A string representing the label for the specified item.</returns>
    protected abstract string GetItemLabel(T item);

    /// Draws the label for a given dropdown item.
    /// <param name="region">The rectangle area within which the label will be drawn.</param>
    /// <param name="item">The dropdown item whose label is to be drawn.</param>
    protected abstract void DrawItemLabel(Rect region, T item);

    /// Determines whether two items of type T are equal.
    /// <param name="item1">The first item to compare.</param>
    /// <param name="item2">The second item to compare.</param>
    /// <returns>true if the items are equal; otherwise, false.</returns>
    protected abstract bool AreItemsEqual(T item1, T item2);
}
