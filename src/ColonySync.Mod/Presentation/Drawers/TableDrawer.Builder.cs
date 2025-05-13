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
using System.Linq;
using UnityEngine;

namespace ColonySync.Mod.Presentation.Drawers;

public abstract partial class TableDrawer<T>
{
    /// <summary>
    ///     Builder class for constructing instances of TableDrawer with configurable columns and
    ///     data.
    /// </summary>
    /// <typeparam name="TBase">The base type of the TableDrawer.</typeparam>
    public class Builder<TBase>(Func<TBase> instantiator) where TBase : TableDrawer<T>
    {
        /// <summary>
        ///     Holds the collection of columns for the table. Each column is represented by a
        ///     <see cref="TableColumn" /> instance, and this list determines the structure and order of
        ///     columns to be displayed in the table.
        /// </summary>
        private readonly List<TableColumn> _columns = [];

        /// <summary>Adds a column to the table with the specified options.</summary>
        /// <param name="columnOptionsAction">
        ///     The action to configure the column options. It takes a
        ///     <see cref="TableColumnOptions" /> instance that can be used to set various properties for the
        ///     column.
        /// </param>
        /// <returns>The <see cref="Builder{TBase}" /> instance, allowing for a fluent API.</returns>
        public Builder<TBase> WithColumn(Action<TableColumnOptions> columnOptionsAction)
        {
            var options = new TableColumnOptions();

            // User configure
            columnOptionsAction(options);

            _columns.Add(new TableColumn(options.Header, options.RelativeWidth, options.Icon, options.SortAction));

            return this;
        }

        /// <summary>Builds an instance of the TableDrawer with the specified data set.</summary>
        /// <param name="dataSet">The data to be displayed in the table.</param>
        /// <returns>A fully constructed instance of the TableDrawer with the provided data set.</returns>
        public TBase Build(IEnumerable<T> dataSet)
        {
            var instance = instantiator();
            instance._columns = _columns;
            instance._columnRegions = new Rect[_columns.Count];
            instance._contents = dataSet.Select(i => new TableEntry(i)).ToList();
            instance._viewportHeight = instance._contents.Count * UiConstants.LineHeight;

            return instance;
        }
    }

    /// <summary>Represents options for configuring a table column.</summary>
    public class TableColumnOptions
    {
        /// <summary>Gets or sets the icon to be displayed in the table column header.</summary>
        /// <value>
        ///     A <see cref="Texture2D" /> that represents the icon. This property is optional and defaults
        ///     to null.
        /// </value>
        public Texture2D? Icon { get; set; }

        /// <summary>
        ///     Gets or sets an action to be invoked to sort data within a table column. The action takes
        ///     two parameters: the current sort order and a read-only list of table entries.
        /// </summary>
        /// <remarks>
        ///     The sort action allows custom sorting logic to be provided, executing when the table needs
        ///     to sort its entries based on user interactions or other triggers.
        /// </remarks>
        public Action<SortOrder, IReadOnlyList<TableEntry>>? SortAction { get; set; }

        /// <summary>Gets or sets the header text for the table column.</summary>
        /// <remarks>
        ///     The header text is displayed at the top of the table column. It provides a title or
        ///     description for the contents of the column.
        /// </remarks>
        public string? Header { get; set; }

        /// <summary>Gets or sets the relative width of a table column.</summary>
        /// <remarks>
        ///     This property specifies the proportion of the total width that this column should occupy.
        ///     For example, if there are two columns with RelativeWidth values of 1 and 2, the first column
        ///     will take up one-third of the total width, and the second column will take up the remaining
        ///     two-thirds.
        /// </remarks>
        public float RelativeWidth { get; set; }

        /// <summary>
        ///     Gets or sets the anchor position for text within the table column. This property
        ///     determines how the text is aligned within the column, such as centered, left-aligned, or
        ///     right-aligned.
        /// </summary>
        public TextAnchor TextAnchor { get; set; }
    }

    /// <summary>
    ///     Represents a column in a table, encapsulating its header, relative width, icon, and sort
    ///     action.
    /// </summary>
    private sealed class TableColumn(
        string? header,
        float relativeWidth,
        Texture2D? icon = null,
        Action<SortOrder, IReadOnlyList<TableEntry>>? sortAction = null)
    {
        /// <summary>Represents the order in which the items in the table column should be sorted.</summary>
        public SortOrder SortOrder { get; set; }

        /// <summary>Gets the header text of the table column.</summary>
        /// <value>A string representing the header text of the column. Can be null.</value>
        public string? Header { get; } = header;

        /// <summary>
        ///     Gets the relative width of the table column. This value is a proportion of the total width
        ///     of the table that the column occupies. It is used to dynamically calculate the column's width
        ///     based on the table's current size.
        /// </summary>
        public float RelativeWidth { get; } = relativeWidth;

        /// <summary>Gets the icon associated with the table column.</summary>
        /// <remarks>
        ///     This property returns a <see cref="Texture2D" /> object that represents the icon displayed
        ///     in the table column header. If no icon was specified during the creation of the
        ///     <see cref="TableColumn" />, this property will return null.
        /// </remarks>
        public Texture2D? Icon { get; } = icon;

        /// <summary>
        ///     Gets the action to be executed when a sorting operation is triggered. The action takes the
        ///     sort order and the list of table entries to be sorted.
        /// </summary>
        public Action<SortOrder, IReadOnlyList<TableEntry>>? SortAction { get; } = sortAction;
    }

    /// <summary>Represents an entry within a table, encapsulating data and visibility properties.</summary>
    /// <typeparam name="T">The type of data stored within the table entry.</typeparam>
    public sealed class TableEntry(T data)
    {
        /// <summary>Gets the data associated with the table entry.</summary>
        /// <remarks>
        ///     This property is initialized with the data passed to the <see cref="TableEntry" />
        ///     constructor and is read-only.
        /// </remarks>
        public T Data { get; } = data;

        /// <summary>Gets or sets a value indicating whether the table entry is visible.</summary>
        /// <value><c>true</c> if the table entry is visible; otherwise, <c>false</c>.</value>
        public bool Visible { get; set; } = true;
    }
}