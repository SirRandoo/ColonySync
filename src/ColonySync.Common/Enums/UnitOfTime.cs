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
using JetBrains.Annotations;

namespace ColonySync.Common.Enums;

/// <summary>
///     Represents a unit of time with a name and scale in seconds.
/// </summary>
/// <remarks>
///     This structure defines a unit of time, such as seconds, minutes, hours, or days, and provides
///     functionality to convert the unit into a corresponding <see cref="TimeSpan" />. Each unit of
///     time is associated with a name and an integer scale that represents the number of seconds
///     in that unit.
/// </remarks>
[PublicAPI]
public readonly record struct UnitOfTime(string Name, int Scale)
{
    /// <summary>
    ///     Represents the unit of time "Seconds" with a scale of 1 second.
    ///     This unit of time is used for time-related calculations or conversions
    ///     where the base unit is expressed in individual seconds.
    /// </summary>
    public static readonly UnitOfTime Seconds = new(Name: "Seconds", Scale: 1);

    /// <summary>
    ///     Represents the "Minutes" unit of time, defined with a scale of 60 seconds.
    /// </summary>
    /// <remarks>
    ///     This is a predefined instance of <see cref="UnitOfTime" /> used to represent
    ///     a time duration equivalent to one minute. It is commonly used to express
    ///     time spans and perform conversions within the system.
    /// </remarks>
    public static readonly UnitOfTime Minutes = new(Name: "Minutes", Scale: 60);

    /// <summary>
    ///     Represents the unit of time "Hours".
    /// </summary>
    /// <remarks>
    ///     - Name: "Hours"
    ///     - Scale: 3,600 seconds
    /// </remarks>
    public static readonly UnitOfTime Hours = new(Name: "Hours", Scale: 3_600);

    /// <summary>
    ///     Represents a unit of time corresponding to "Days", with a scale of 86,400 seconds.
    /// </summary>
    /// <remarks>
    ///     The "Days" unit is used to define longer duration periods as part of the UnitOfTime struct.
    /// </remarks>
    public static readonly UnitOfTime Days = new(Name: "Days", Scale: 86_400);

    /// <summary>
    ///     Converts the current UnitOfTime instance to a TimeSpan representation based on its scale.
    /// </summary>
    /// <returns>
    ///     A TimeSpan object representing the duration defined by the UnitOfTime's scale in seconds.
    /// </returns>
    public TimeSpan ToTimeSpan()
    {
        return TimeSpan.FromSeconds(Scale);
    }

    /// <summary>
    ///     Converts the current UnitOfTime instance to a TimeSpan representation based on its scale.
    /// </summary>
    /// <returns>
    ///     A TimeSpan object representing the duration defined by the UnitOfTime's scale in seconds.
    /// </returns>
    public TimeSpan ToTimeSpan(double seconds)
    {
        return TimeSpan.FromSeconds(seconds * Scale);
    }

    /// <summary>
    ///     Attempts to parse the provided unit name into a corresponding UnitOfTime instance.
    /// </summary>
    /// <param name="unitName">
    ///     The name of the unit to parse (e.g., "Seconds", "Minutes", "Hours", "Days").
    /// </param>
    /// <param name="unit">
    ///     When the method returns, contains the corresponding UnitOfTime instance if the parsing is successful, or null if it
    ///     fails.
    /// </param>
    /// <returns>
    ///     True if the parsing is successful; otherwise, false.
    /// </returns>
    public static bool TryParse(string unitName, out UnitOfTime unit)
    {
        if (string.IsNullOrWhiteSpace(unitName))
        {
            unit = default;
            return false;
        }

        if (unitName.Equals(Seconds.Name, StringComparison.OrdinalIgnoreCase))
        {
            unit = Seconds;
            return true;
        }

        if (unitName.Equals(Minutes.Name, StringComparison.OrdinalIgnoreCase))
        {
            unit = Minutes;
            return true;
        }

        if (unitName.Equals(Hours.Name, StringComparison.OrdinalIgnoreCase))
        {
            unit = Hours;

            return true;
        }

        if (unitName.Equals(Days.Name, StringComparison.OrdinalIgnoreCase))
        {
            unit = Days;

            return true;
        }

        unit = default;
        return false;
    }

    /// <summary>
    ///     Retrieves the names of all defined units of time.
    /// </summary>
    /// <returns>
    ///     An array of strings representing the names of all predefined UnitOfTime instances.
    /// </returns>
    public static string[] GetNames()
    {
        return [Seconds.Name, Minutes.Name, Hours.Name, Days.Name];
    }
}