namespace ColonySync.Common;

/// <summary>
///     Represents a range with a lower and an upper bound within the range of an unsigned short
///     integer.
/// </summary>
/// <param name="minimum">The lower bound of the range.</param>
/// <param name="maximum">The upper bound of the range.</param>
public struct UShortRange(ushort minimum = ushort.MinValue, ushort maximum = ushort.MaxValue)
{
    /// <summary>The lower bound within the range of ushort values.</summary>
    public ushort Minimum = minimum;

    /// <summary>The upper bound of the range.</summary>
    public ushort Maximum = maximum;
}