using System;
using System.Collections.Generic;
using ColonySync.Mod.Presentation.Drawers;
using UnityEngine;

namespace ColonySync.Mod.Presentation.Dialogs;

/// <summary>
///     A concrete implementation of the DropdownDialog class that specifically handles string
///     options.
/// </summary>
public sealed class StringDropdownDialog : DropdownDialog<string>
{
    /// Represents a dialog displaying a dropdown list of string options.
    public StringDropdownDialog(Rect parentRegion, string current, IReadOnlyList<string> allOptions,
        Action<string> setter) : base(
        parentRegion,
        current,
        allOptions,
        setter
    )
    {
    }

    /// <summary>Retrieves the label for a given item.</summary>
    /// <param name="item">The item for which to get the label.</param>
    /// <returns>The label of the item as a string.</returns>
    protected override string GetItemLabel(string item)
    {
        return item;
    }

    /// <summary>Draws the label of an individual item within the dropdown dialog.</summary>
    /// <param name="region">The rectangular area where the item label should be drawn.</param>
    /// <param name="item">The item whose label is to be drawn.</param>
    protected override void DrawItemLabel(Rect region, string item)
    {
        LabelDrawer.DrawLabel(region, item);
    }

    /// Determines whether two items of type T are equal.
    /// <param name="item1">The first item to compare.</param>
    /// <param name="item2">The second item to compare.</param>
    /// <returns>True if the specified items are equal; otherwise, false.</returns>
    protected override bool AreItemsEqual(string item1, string item2)
    {
        return string.Equals(item1, item2, StringComparison.InvariantCultureIgnoreCase);
    }
}
