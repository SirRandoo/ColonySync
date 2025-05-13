using JetBrains.Annotations;

namespace ColonySync.Common;

/// <summary>
///     Represents an inclusive range with specified minimum and maximum values within a
///     <see cref="byte" />'s valid range.
/// </summary>
/// <param name="minimum">The inclusive lower bound of the range.</param>
/// <param name="maximum">The inclusive upper bound of the range.</param>
[PublicAPI]
public struct ByteRange(byte minimum = byte.MinValue, byte maximum = byte.MaxValue)
{
    /// <summary>Represents the lower bound of the range.</summary>
    public byte Minimum = minimum;

    /// <summary>The upper bound of the range.</summary>
    public byte Maximum = maximum;
}