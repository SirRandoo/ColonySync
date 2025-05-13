namespace ColonySync.Mod.Shared.Serializers;

/// <summary>
///     Provides a mechanism to retrieve an <see cref="IDataSerializer" /> implementation
///     based on a specified format.
/// </summary>
public interface IDataSerializerFactory
{
    /// <summary>Retrieves an <see cref="IDataSerializer" /> instance that matches the given format.</summary>
    /// <param name="format">
    ///     The desired data format, such as <see cref="DataFormat.Json" /> or
    ///     <see cref="DataFormat.Toml" />.
    /// </param>
    /// <returns>
    ///     An instance of <see cref="IDataSerializer" /> capable of handling the specified format. If
    ///     no matching serializer is found, this method may throw an exception or return
    ///     <see langword="null" />.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     Thrown when the provided format is not recognized or supported
    ///     by the factory.
    /// </exception>
    IDataSerializer GetSerializer(DataFormat format);
}