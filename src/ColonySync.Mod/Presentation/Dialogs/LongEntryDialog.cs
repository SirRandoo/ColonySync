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

using System.Globalization;
using JetBrains.Annotations;

namespace ColonySync.Mod.Presentation.Dialogs;

/// <summary>Represents a dialog window for entering a long integer within a specified range.</summary>
[PublicAPI]
public sealed class LongEntryDialog(long minimum = 0, long maximum = long.MaxValue)
    : NumberEntryDialog<long>(minimum, maximum)
{
    /// <inheritdoc />
    protected override string FormatNumber(long value)
    {
        return value.ToString(format: "N0", NumberFormatInfo.CurrentInfo);
    }

    /// <inheritdoc />
    protected override bool TryParseNumber(string value, out long number)
    {
        return long.TryParse(
            value,
            NumberStyles.Integer | NumberStyles.AllowCurrencySymbol | NumberStyles.AllowExponent |
            NumberStyles.AllowThousands,
            NumberFormatInfo.CurrentInfo,
            out number
        );
    }
}