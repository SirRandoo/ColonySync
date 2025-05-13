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
using ColonySync.Common.Enums;
using JetBrains.Annotations;

namespace ColonySync.Common.Extensions;

/// <summary>Provides extension methods for <see cref="TimeSpan" />.</summary>
[PublicAPI]
public static class TimeSpanExtensions
{
    /// <summary>
    ///     Returns the largest <see cref="UnitOfTime" /> for a <see cref="TimeSpan" />. The largest
    ///     unit is calculated by checking if a <see cref="UnitOfTime" /> is non-zero, starting from the
    ///     longest period of time to the shortest period of time.
    /// </summary>
    /// <param name="span">
    ///     The <see cref="TimeSpan" /> for which to determine the largest
    ///     <see cref="UnitOfTime" />.
    /// </param>
    /// <returns>The largest <see cref="UnitOfTime" /> for the given <see cref="TimeSpan" />.</returns>
    [PublicAPI]
    public static UnitOfTime GetLongestTimePeriod(this TimeSpan span)
    {
        if (span.TotalDays >= 1) return UnitOfTime.Days;

        if (span.TotalHours >= 1) return UnitOfTime.Hours;

        return span.TotalMinutes >= 1 ? UnitOfTime.Minutes : UnitOfTime.Seconds;
    }

    /// <summary>Returns a stringified <see cref="TimeSpan" />.</summary>
    /// <param name="span">The time span being turned into a string.</param>
    /// <param name="unit">The unit of time to convert the time span into.</param>
    /// <exception cref="ArgumentOutOfRangeException">The unit of time specified wasn't supported.</exception>
    /// <returns>A string representing the time span in the specified unit of time.</returns>
    [PublicAPI]
    public static string ToString(this TimeSpan span, UnitOfTime unit)
    {
        if (unit == UnitOfTime.Seconds) return span.TotalSeconds.ToString("N0");
        if (unit == UnitOfTime.Minutes) return span.TotalMinutes.ToString("N0");
        if (unit == UnitOfTime.Hours) return span.TotalHours.ToString("N0");
        if (unit == UnitOfTime.Days) return span.TotalDays.ToString("N0");

        return "0";
    }
}