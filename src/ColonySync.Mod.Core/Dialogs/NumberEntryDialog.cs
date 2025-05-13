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
using ColonySync.Mod.Presentation;
using ColonySync.Mod.Presentation.Drawers;
using JetBrains.Annotations;
using UnityEngine;
using Verse;

namespace ColonySync.Mod.Core.Dialogs;

/// <summary>Represents an abstract base class for dialogs that prompt the user to enter a number within a specified range.</summary>
/// <typeparam name="T">
///     A struct that implements <see cref="System.IComparable" /> representing the type of number to be
///     entered.
/// </typeparam>
[PublicAPI]
public abstract class NumberEntryDialog<T> : Window where T : struct, IComparable
{
    private readonly T _maximum;
    private readonly T _minimum;

    private string _buffer = null!;
    private BufferValidityCode _bufferStatus = BufferValidityCode.None;
    private T _current;

    protected NumberEntryDialog(T minimum, T maximum)
    {
        _minimum = minimum;
        _maximum = maximum;

        doCloseX = false;
        closeOnCancel = true;
        doCloseButton = false;
        layer = WindowLayer.Dialog;
    }

    /// <inheritdoc />
    public override Vector2 InitialSize => new(x: 300, y: 140);

    /// <summary>Occurs when a number is entered in the number entry dialog.</summary>
    public event EventHandler<T> NumberEntered = null!;

    /// <inheritdoc />
    public override void DoWindowContents(Rect inRect)
    {
        var fieldRegion = new Rect(x: 0f, y: 0f, inRect.width, UiConstants.LineHeight);
        var rangeRegion = new Rect(x: 0f, fieldRegion.height, inRect.width, UiConstants.LineHeight);
        var buttonRegion = new Rect(x: 0f, rangeRegion.y + rangeRegion.height, inRect.width, UiConstants.LineHeight);

        GUI.BeginGroup(inRect);

        GUI.BeginGroup(fieldRegion);
        DrawEntryField(fieldRegion);
        GUI.EndGroup();

        GUI.BeginGroup(rangeRegion);
        DrawValidityStatus(rangeRegion.AtZero());
        GUI.EndGroup();

        GUI.BeginGroup(buttonRegion);
        DrawDialogButtons(buttonRegion.AtZero());
        GUI.EndGroup();

        GUI.EndGroup();
    }

    /// <summary>Draws an entry field for user input within the specified region.</summary>
    /// <param name="region">The screen position and size of the entry field.</param>
    private void DrawEntryField(Rect region)
    {
        GUI.color = _bufferStatus is BufferValidityCode.None ? Color.white : Color.red;

        if (FieldDrawer.DrawTextField(region, _buffer, out var newContent))
        {
            _buffer = newContent;
            _bufferStatus = GetBufferValidity();
        }

        GUI.color = Color.white;
    }

    /// Draws the validity status message within a specified region.
    /// <param name="region">The region in which to draw the validity status message.</param>
    private void DrawValidityStatus(Rect region)
    {
        Color statusColor;
        string statusText;

        switch (_bufferStatus)
        {
            case BufferValidityCode.TooHigh:
                statusColor = Color.red;
                statusText = string.Format(KitTranslations.ValueTooHighInputError, FormatNumber(_maximum));

                break;
            case BufferValidityCode.TooLow:
                statusColor = Color.red;
                statusText = string.Format(KitTranslations.ValueTooLowInputError, FormatNumber(_minimum));

                break;
            case BufferValidityCode.NaN:
                statusColor = Color.red;
                statusText = KitTranslations.NaNInputError;

                break;
            default:
                statusColor = new Color(r: 0.72f, g: 0.72f, b: 0.72f);

                statusText = string.Format(
                    KitTranslations.ValueOutOfRangeInputError, FormatNumber(_minimum),
                    FormatNumber(_maximum)
                );

                break;
        }

        if (string.IsNullOrEmpty(statusText)) return;

        LabelDrawer.DrawLabel(region, statusText, statusColor, TextAnchor.UpperCenter, GameFont.Tiny);
    }

    /// <summary>Draws the confirm and cancel buttons for the dialog within the specified region.</summary>
    /// <param name="region">The region within which to draw the buttons.</param>
    private void DrawDialogButtons(Rect region)
    {
        var confirmRegion = new Rect(x: 0f, y: 0f, Mathf.FloorToInt(region.width * 0.5f) - 5f, region.height);
        var cancelRegion = new Rect(confirmRegion.width + 10f, y: 0f, confirmRegion.width, region.height);

        if (Widgets.ButtonText(confirmRegion, KitTranslations.CommonTextConfirm))
        {
            OnNumberEntered(_current);

            Close();
        }

        if (Widgets.ButtonText(cancelRegion, KitTranslations.CommonTextCancel)) Close();
    }

    /// <inheritdoc />
    public override void OnAcceptKeyPressed()
    {
        OnNumberEntered(_current);

        Close();
    }

    /// <inheritdoc />
    public override void PostOpen()
    {
        _buffer = FormatNumber(_current);
        _bufferStatus = GetBufferValidity();
    }

    /// <summary>
    ///     Validates the content of the input buffer, checking if it can be parsed into a number of type
    ///     <typeparamref name="T" /> and if it falls within the specified range.
    /// </summary>
    /// <return>
    ///     A code representing the validity of the input buffer: None if valid, NaN if not a number, TooHigh if above the
    ///     maximum, and TooLow if below the minimum.
    /// </return>
    private BufferValidityCode GetBufferValidity()
    {
        if (!TryParseNumber(_buffer, out var number)) return BufferValidityCode.NaN;

        _current = number;

        if (_current.CompareTo(_maximum) > 0) return BufferValidityCode.TooHigh;

        return _current.CompareTo(_minimum) < 0 ? BufferValidityCode.TooLow : BufferValidityCode.None;
    }

    /// <summary>Formats a number of type <typeparamref name="T" /> as a string, using appropriate formatting rules.</summary>
    /// <param name="value">The numeric value to format.</param>
    /// <returns>A string representation of the numeric value.</returns>
    protected abstract string FormatNumber(T value);

    /// <summary>Attempts to parse a string into a number of type <typeparamref name="T" />.</summary>
    /// <param name="value">The string representation of the number to parse.</param>
    /// <param name="number">The parsed number if the operation is successful.</param>
    /// <returns>True if the string was successfully parsed; otherwise, false.</returns>
    protected abstract bool TryParseNumber(string value, out T number);

    /// <summary>Raises the <see cref="NumberEntered" /> event with the specified number value.</summary>
    /// <param name="value">The number entered by the user.</param>
    protected virtual void OnNumberEntered(T value)
    {
        NumberEntered?.Invoke(this, value);
    }

    /// <summary>Enumerates the possible statuses of the buffer validity during user number entry.</summary>
    private enum BufferValidityCode
    {
        /// <summary>Indicates that the buffer content is currently neutral, meaning it has not been validated.</summary>
        None,

        /// <summary>Indicates that the input value is higher than the maximum allowable limit.</summary>
        TooHigh,

        /// <summary>Indicates that the entered value is below the acceptable minimum threshold.</summary>
        TooLow,

        /// <summary>
        ///     Indicates that the buffer value is not a number (NaN), meaning the input provided is not a valid numeric
        ///     value.
        /// </summary>
        NaN
    }
}