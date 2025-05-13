﻿// MIT License
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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using ColonySync.Mod.Presentation.Extensions;
using JetBrains.Annotations;
using UnityEngine;
using Verse;

namespace ColonySync.Mod.Presentation.Drawers;

// TODO: Number fields should be decorated with an error icon to better indicate to the user that the text in the field isn't a valid number.

/// <summary>
///     Provides various static methods for drawing input fields within a graphical user
///     interface.
/// </summary>
[PublicAPI]
public static class FieldDrawer
{
    private const NumberStyles BaseNumberStyles =
        NumberStyles.AllowThousands | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingWhite |
        NumberStyles.AllowLeadingWhite;

    private const NumberStyles FloatNumberStyles = BaseNumberStyles | NumberStyles.AllowDecimalPoint;

    /// <summary>Draws a text field on the specified region of the current drawing canvas.</summary>
    /// <param name="region">The region of the drawing canvas to draw the text field in.</param>
    /// <param name="value">The text to draw within the text field.</param>
    /// <param name="newValue">
    ///     The new text if the user modified the text field, or <see langword="null" />
    ///     if the user hasn't modified the field's contents.
    /// </param>
    /// <returns>Whether the text within the text field was modified by the user.</returns>
    public static bool DrawTextField(Rect region, string? value, [NotNullWhen(true)] out string? newValue)
    {
        var content = Widgets.TextField(region, value);
        newValue = string.Equals(value, content, StringComparison.InvariantCulture) ? null : content;

        return newValue != null;
    }

    /// <summary>Draws a number field on the specified region of the current drawing canvas.</summary>
    /// <param name="region">The region of the drawing canvas to draw the number field in.</param>
    /// <param name="value">
    ///     The integer that was parsed from the user's input, or <c>0</c> if the value
    ///     within the field could not be parsed into a valid integer.
    /// </param>
    /// <param name="buffer">
    ///     A reference to the text being displayed within the text field. The string
    ///     stored at the reference will be updated when the user changes the contents of the text field.
    /// </param>
    /// <param name="bufferValid">
    ///     A reference to a boolean that dictates whether the contents of the text
    ///     field is a valid integer. If the contents aren't a valid integer, the text field will be
    ///     decorated with information to inform the user the text field's content isn't a valid integer.
    /// </param>
    /// <param name="minimum">The minimum integer <see cref="value" /> can be.</param>
    /// <param name="maximum">The maximum integer <see cref="value" /> can be.</param>
    /// <returns>Whether a number was successfully parsed from the input.</returns>
    public static bool DrawNumberField(Rect region, out int value, ref string? buffer, ref bool bufferValid,
        int minimum = int.MinValue, int maximum = int.MaxValue)
    {
        GUI.backgroundColor = bufferValid ? Color.white : Color.red;

        if (!DrawTextField(region, buffer, out var newValue))
        {
            value = 0;
            GUI.backgroundColor = Color.white;

            return false;
        }

        buffer = newValue;

        if (int.TryParse(buffer, BaseNumberStyles, NumberFormatInfo.CurrentInfo, out var result))
        {
            value = Mathf.Clamp(result, minimum, maximum);
            bufferValid = true;

            GUI.backgroundColor = Color.white;

            return true;
        }

        value = 0;
        bufferValid = false;

        GUI.backgroundColor = Color.white;

        return false;
    }

    /// <summary>Draws a number field on the specified region of the current drawing canvas.</summary>
    /// <param name="region">The region of the drawing canvas to draw the number field in.</param>
    /// <param name="value">
    ///     The floating point value that was parsed from the user's input, or <c>0</c> if
    ///     the value within the field could not be parsed into a valid floating point value.
    /// </param>
    /// <param name="buffer">
    ///     A reference to the text being displayed within the text field. The string
    ///     stored at the reference will be updated when the user changes the contents of the text field.
    /// </param>
    /// <param name="bufferValid">
    ///     A reference to a boolean that dictates whether the contents of the text
    ///     field is a valid floating point value. If the contents isn't a valid floating point value, the
    ///     text field will be decorated with information to inform the user the text field's content isn't
    ///     a valid floating point value.
    /// </param>
    /// <param name="minimum">The minimum floating point value <see cref="value" /> can be.</param>
    /// <param name="maximum">The maximum floating point value <see cref="value" /> can be.</param>
    /// <returns>Whether a number was successfully parsed from the input.</returns>
    public static bool DrawNumberField(
        Rect region,
        out float value,
        ref string? buffer,
        ref bool bufferValid,
        float minimum = float.MinValue,
        float maximum = float.MaxValue
    )
    {
        GUI.backgroundColor = bufferValid ? Color.white : Color.red;

        if (!DrawTextField(region, buffer, out var newValue))
        {
            value = 0;
            GUI.backgroundColor = Color.white;

            return false;
        }

        buffer = newValue;

        if (float.TryParse(buffer, FloatNumberStyles, NumberFormatInfo.CurrentInfo, out var result))
        {
            value = Mathf.Clamp(result, minimum, maximum);
            bufferValid = true;

            GUI.backgroundColor = Color.white;

            return true;
        }

        value = 0;
        bufferValid = false;

        GUI.backgroundColor = Color.white;

        return false;
    }

    /// <summary>Draws a number field on the specified region of the current drawing canvas.</summary>
    /// <param name="region">The region of the drawing canvas to draw the number field in.</param>
    /// <param name="value">
    ///     The floating point value that was parsed from the user's input, or <c>0</c> if
    ///     the value within the field could not be parsed into a valid floating point value.
    /// </param>
    /// <param name="buffer">
    ///     A reference to the text being displayed within the text field. The string
    ///     stored at the reference will be updated when the user changes the contents of the text field.
    /// </param>
    /// <param name="bufferValid">
    ///     A reference to a boolean that dictates whether the contents of the text
    ///     field is a valid floating point value. If the contents isn't a valid floating point value, the
    ///     text field will be decorated with information to inform the user the text field's content isn't
    ///     a valid floating point value.
    /// </param>
    /// <returns>Whether a number was successfully parsed from the input.</returns>
    public static bool DrawNumberField(Rect region, out double value, ref string buffer, ref bool bufferValid)
    {
        GUI.backgroundColor = bufferValid ? Color.white : Color.red;

        if (!DrawTextField(region, buffer, out var newValue))
        {
            value = 0;
            GUI.backgroundColor = Color.white;

            return false;
        }

        buffer = newValue;

        if (double.TryParse(buffer, FloatNumberStyles, NumberFormatInfo.CurrentInfo, out var result))
        {
            value = result;
            bufferValid = true;

            GUI.backgroundColor = Color.white;

            return true;
        }

        value = 0;
        bufferValid = false;

        GUI.backgroundColor = Color.white;

        return false;
    }

    /// <summary>Draws a URI input field on the specified region of the current drawing canvas.</summary>
    /// <param name="region">The region of the drawing canvas to draw the URI input field in.</param>
    /// <param name="value">
    ///     The resulting URI if the user entered a valid URI and modified the field's
    ///     contents, or null if the user hasn't modified the field or entered an invalid URI.
    /// </param>
    /// <param name="buffer">A buffer to hold the current text representation of the URI.</param>
    /// <param name="bufferValid">Indicates whether the current buffer contains a valid URI.</param>
    /// <returns>Whether the URI input field was modified by the user and contains a valid URI.</returns>
    public static bool DrawUri(Rect region, [NotNullWhen(true)] out Uri? value, ref string? buffer,
        ref bool bufferValid)
    {
        var fieldRegion = region;

        if (!bufferValid)
        {
            var warningIcon = RectExtensions.IconRect(region.x + region.width - region.height, region.y, region.height,
                region.height, margin: 6f);
            fieldRegion.width = fieldRegion.width - region.height - 2f;

            IconDrawer.DrawIcon(warningIcon, Icons.Get(IconCategory.Solid, icon: "TriangleExclamation"), UxColors.RedishPink);

            // TODO: This is pretty broad, and doesn't tell the user anything specific.
            TooltipHandler.TipRegion(warningIcon, UxLocale.InvalidUrlColored);
        }

        GUI.backgroundColor = bufferValid ? Color.white : Color.red;

        if (!DrawTextField(fieldRegion, buffer, out var newValue))
        {
            value = null;
            GUI.backgroundColor = Color.white;

            return false;
        }

        buffer = newValue;

        if (Uri.TryCreate(buffer, UriKind.Absolute, out value)) // TODO: Replace this with something more optimal
        {
            bufferValid = true;
            GUI.backgroundColor = Color.white;

            return true;
        }

        value = null;
        bufferValid = false;

        GUI.backgroundColor = Color.white;

        return false;
    }
}
