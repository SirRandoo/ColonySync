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
using ColonySync.Common.Interfaces;
using ColonySync.Mod.Presentation.Extensions;
using JetBrains.Annotations;
using UnityEngine;
using Verse;

namespace ColonySync.Mod.Presentation.Drawers;

/// <summary>Provides functionality for rendering dropdown menus in the user interface.</summary>
[PublicAPI]
public static class DropdownDrawer
{
    /// <summary>Draws a dropdown at the given region.</summary>
    /// <param name="region">The region to draw the dropdown in.</param>
    /// <param name="current">The current item being displayed in the dropdown display.</param>
    /// <param name="allOptions">A list containing all the available options a user can select.</param>
    /// <param name="setterFunc">
    ///     An action that's called when the user selects an option in the dropdown
    ///     menu.
    /// </param>
    public static void Draw(Rect region, string current, IReadOnlyList<string> allOptions, Action<string> setterFunc)
    {
        if (DrawButton(region, current))
            Find.WindowStack.Add(new StringDropdownDialog(GetDialogPosition(region), current, allOptions, setterFunc));
    }

    /// <summary>Draws a dropdown at the given region.</summary>
    /// <param name="region">The region to draw the dropdown in.</param>
    /// <param name="current">The current item being displayed in the dropdown display.</param>
    /// <param name="allOptions">A list containing all the available options a user can select.</param>
    /// <param name="setterFunc">
    ///     An action that's called when the user selects an option in the dropdown
    ///     menu.
    /// </param>
    /// <typeparam name="T">The type of the items in the dropdown, which must implement IIdentifiable.</typeparam>
    public static void Draw<T>(Rect region, T current, IReadOnlyList<IIdentifiable> allOptions,
        Action<IIdentifiable> setterFunc) where T : IIdentifiable
    {
        if (DrawButton(region, current.Name))
            Find.WindowStack.Add(new IdentifiableDropdownDialog(GetDialogPosition(region), current, allOptions,
                setterFunc));
    }

    /// <summary>Calculates the position of the dialog based on the given parent region.</summary>
    /// <param name="parentRegion">The parent region from which to base the dialog's position.</param>
    /// <return>A Rect structure representing the position of the dialog.</return>
    private static Rect GetDialogPosition(Rect parentRegion)
    {
        return UI.GUIToScreenRect(parentRegion);
    }

    /// <summary>Draws a button with the specified label at the given region.</summary>
    /// <param name="region">The region to draw the button in.</param>
    /// <param name="label">The label text to display on the button.</param>
    /// <returns>Returns true if the button is clicked; otherwise, false.</returns>
    public static bool DrawButton(Rect region, string label)
    {
        var labelRegion = new Rect(region.x + 5f, region.y, region.width - region.height, region.height);
        var iconRegion = RectExtensions.IconRect(region.x + region.width - region.height, region.y, region.height,
            region.height, UiConstants.LineHeight * 0.3f);

        Widgets.DrawLightHighlight(region);

        GUI.color = Color.grey;
        Widgets.DrawBox(region);
        GUI.color = Color.white;

        LabelDrawer.DrawLabel(labelRegion, label);

        GUI.DrawTexture(iconRegion, Icons.Get(IconCategory.Solid, "AngleDown"));

        Widgets.DrawHighlightIfMouseover(region);

        return Widgets.ButtonInvisible(region);
    }

    /// <summary>
    ///     Represents a dropdown dialog window in the UI.
    /// </summary>
    /// <typeparam name="T">The type of the options presented in the dropdown dialog.</typeparam>
    internal abstract class DropdownDialog<T>(
        Rect parentRegion,
        T current,
        IReadOnlyList<T> allOptions,
        Action<T> setter)
        : Dialogs.DropdownDialog<T>(parentRegion, current, allOptions, setter)
    {
        /// <inheritdoc />
        protected override float Margin => 5f;

        /// <summary>
        ///     Sets the initial size and position of the dropdown dialog window.
        /// </summary>
        /// <remarks>
        ///     This method calculates the position and size of the window based on
        ///     the dropdown region and whether the dropdown is reversed or not, expanding
        ///     the window to provide a margin around the dropdown region.
        /// </remarks>
        protected override void SetInitialSizeAndPosition()
        {
            base.SetInitialSizeAndPosition();

            var yPosition = IsReversed ? DropdownRegion.y - ViewHeight : DropdownRegion.y;

            windowRect = new Rect(DropdownRegion.x, yPosition, DropdownRegion.width,
                ViewHeight + DropdownRegion.height);
            windowRect = windowRect.ExpandedBy(5f);
        }

        /// <summary>
        ///     Draws the contents of the window, including the dropdown and its options.
        /// </summary>
        /// <param name="inRect">The rectangle defining the area of the window.</param>
        public override void DoWindowContents(Rect inRect)
        {
            var dropdownRegion = new Rect(x: 0f, IsReversed ? inRect.height - UiConstants.LineHeight : 0f, inRect.width,
                UiConstants.LineHeight);
            var dropdownOptionsRegion = new Rect(x: 0f, IsReversed ? 0f : DropdownRegion.height, inRect.width,
                inRect.height - UiConstants.LineHeight);

            GUI.BeginGroup(inRect);

            GUI.BeginGroup(dropdownRegion);
            DrawDropdown(dropdownRegion.AtZero());
            GUI.EndGroup();

            Widgets.DrawLightHighlight(dropdownOptionsRegion);
            Widgets.DrawLightHighlight(dropdownOptionsRegion);
            Widgets.DrawLightHighlight(dropdownOptionsRegion);

            GUI.BeginGroup(dropdownOptionsRegion);
            DrawDropdownOptions(dropdownOptionsRegion.AtZero());
            GUI.EndGroup();

            GUI.EndGroup();
        }

        /// <summary>Draws a dropdown at the specified region.</summary>
        /// <param name="region">The region where the dropdown will be rendered.</param>
        private void DrawDropdown(Rect region)
        {
            if (DrawButton(region, GetItemLabel(CurrentOption))) Close();
        }
    }

    /// <summary>Represents a dialog window for selecting a string option from a dropdown menu.</summary>
    private sealed class StringDropdownDialog : DropdownDialog<string>
    {
        /// <summary>Displays a dialog with a dropdown menu for string selection.</summary>
        /// <param name="parentRegion">The parent region for the dialog.</param>
        /// <param name="current">The current string item being displayed in the dropdown.</param>
        /// <param name="allOptions">A list containing all available string options for selection.</param>
        /// <param name="setter">
        ///     An action that's called when the user selects a string option from the
        ///     dropdown.
        /// </param>
        public StringDropdownDialog(Rect parentRegion, string current, IReadOnlyList<string> allOptions,
            Action<string> setter) : base(
            parentRegion,
            current,
            allOptions,
            setter
        )
        {
        }

        /// <summary>Retrieves the label for a specific item to be displayed in the dropdown.</summary>
        /// <param name="item">The item whose label is to be retrieved.</param>
        /// <return>The label as a string for the specified item.</return>
        protected override string GetItemLabel(string item)
        {
            return item;
        }

        /// <summary>
        ///     Draws a label for a dropdown item at the specified region.
        /// </summary>
        /// <param name="region">The region to draw the label in.</param>
        /// <param name="item">The item being displayed in the dropdown.</param>
        protected override void DrawItemLabel(Rect region, string item)
        {
            LabelDrawer.DrawLabel(region, item);
        }

        /// <summary>
        ///     Determines whether two items of type string are equal using an invariant culture
        ///     comparison.
        /// </summary>
        /// <param name="item1">The first string item to compare.</param>
        /// <param name="item2">The second string item to compare.</param>
        /// <returns>True if the items are equal, otherwise false.</returns>
        protected override bool AreItemsEqual(string item1, string item2)
        {
            return string.Equals(item1, item2, StringComparison.InvariantCultureIgnoreCase);
        }
    }

    /// <summary>A dropdown dialog specialization for displaying a list of identifiable items.</summary>
    private sealed class IdentifiableDropdownDialog : DropdownDialog<IIdentifiable>
    {
        /// <summary>
        ///     A dropdown dialog specifically designed for items implementing the IIdentifiable interface.
        /// </summary>
        /// <param name="parentRegion">The region where the dialog will be drawn.</param>
        /// <param name="current">The current item selected in the dropdown.</param>
        /// <param name="allOptions">A list of all available options for selection.</param>
        /// <param name="setter">An action to be called when an item is selected.</param>
        public IdentifiableDropdownDialog(Rect parentRegion, IIdentifiable current,
            IReadOnlyList<IIdentifiable> allOptions, Action<IIdentifiable> setter) : base(
            parentRegion,
            current,
            allOptions,
            setter
        )
        {
        }

        /// <summary>Retrieves the label of a given item.</summary>
        /// <param name="item">The item for which to get the label.</param>
        /// <return>The label of the item.</return>
        protected override string GetItemLabel(IIdentifiable item)
        {
            return item.Name;
        }

        /// <summary>Renders the label for a specified item within a given region.</summary>
        /// <param name="region">The region where the item label will be drawn.</param>
        /// <param name="item">The item whose label needs to be displayed.</param>
        protected override void DrawItemLabel(Rect region, IIdentifiable item)
        {
            LabelDrawer.DrawLabel(region, item.Name);
        }

        /// <summary>Determines if two IIdentifiable items are equal by comparing their Id properties.</summary>
        /// <param name="item1">The first item to compare.</param>
        /// <param name="item2">The second item to compare.</param>
        /// <returns>True if the Id properties of both items are equal; otherwise, false.</returns>
        protected override bool AreItemsEqual(IIdentifiable item1, IIdentifiable item2)
        {
            return string.Equals(item1.Id, item2.Id, StringComparison.Ordinal);
        }
    }
}
