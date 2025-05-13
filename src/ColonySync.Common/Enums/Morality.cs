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

using JetBrains.Annotations;

namespace ColonySync.Common.Enums;

/// <summary>
///     Represents a moral alignment defined by a specific numerical value on a moral spectrum.
/// </summary>
/// <remarks>
///     This struct encapsulates varying degrees of morality, with predefined levels ranging
///     from "Super bad" to "Super good," each associated with a unique numerical value.
/// </remarks>
/// <param name="Value">
///     The numerical representation of the morality, where negative values indicate negative morals,
///     zero indicates neutrality, and positive values signify positive morals.
/// </param>
[PublicAPI]
public readonly record struct Morality(sbyte Value)
{
    /// <summary>
    ///     Represents a predefined morality level categorized as "SuperBad" with an assigned value of -4.
    /// </summary>
    public static readonly Morality SuperBad = new(Value: -4);

    /// <summary>
    ///     Represents a moral state considered strongly negative, with a defined value of -3.
    /// </summary>
    public static readonly Morality ReallyBad = new(Value: -3);

    /// <summary>
    ///     Represents a morality level indicating a value of "Very bad" with an associated numeric value of -2.
    ///     Typically used to denote a significantly negative state or action in the context of moral evaluation.
    /// </summary>
    public static readonly Morality VeryBad = new(Value: -2);

    /// <summary>
    ///     Represents a negative morality state categorized as "Bad."
    ///     Denotes a mild level of immorality or unfavorable behavior.
    /// </summary>
    public static readonly Morality Bad = new(Value: -1);

    /// <summary>
    ///     Represents a neutral morality state with a defined value of 0.
    /// </summary>
    public static readonly Morality Neutral = new(Value: 0);

    /// <summary>
    ///     Represents a predefined <see cref="Morality" /> instance that signifies a positive moral alignment.
    /// </summary>
    /// <remarks>
    ///     This instance is characterized by a value of 1.
    /// </remarks>
    public static readonly Morality Good = new(Value: 1);

    /// <summary>
    ///     Represents a morality level indicating that something is "Very good".
    /// </summary>
    /// <remarks>
    ///     The "VeryGood" morality level has an associated value of 2 and signifies a positive moral standing that is above
    ///     "Good" but below "Really good".
    /// </remarks>
    public static readonly Morality VeryGood = new(Value: 2);

    /// <summary>
    ///     Represents a morality level categorized as "Really good."
    /// </summary>
    /// <remarks>
    ///     This member is part of a predefined set of morality levels within the <c>Morality</c> structure.
    ///     The associated <c>Value</c> for this level is 3.
    /// </remarks>
    public static readonly Morality ReallyGood = new(Value: 3);

    /// <summary>
    ///     Represents a predefined instance of the <see cref="Morality" /> struct signifying the highest positive moral value,
    ///     "Super good," with a value of 4.
    /// </summary>
    public static readonly Morality SuperGood = new(Value: 4);

    /// <summary>
    ///     Retrieves an array containing all possible morality values.
    /// </summary>
    /// <returns>
    ///     An array of <see cref="Morality" /> representing the full range of defined morality values.
    /// </returns>
    public static Morality[] ListMoralities()
    {
        return [SuperBad, ReallyBad, VeryBad, Bad, Neutral, Good, VeryGood, ReallyGood, SuperGood];
    }
}