namespace ColonySync.Mod.Domain.Serializers;

/// <summary>
///     Specifies the available data formats for serialization and deserialization operations.
/// </summary>
public enum DataFormat
{
    /// <summary>Represents the TOML data format.</summary>
    /// <remarks>
    ///     This format is used for serializing and deserializing data in the TOML (Tom's Obvious,
    ///     Minimal Language) format, which is commonly utilized for configuration files and supports
    ///     hierarchical data serialization.
    /// </remarks>
    Toml,

    /// <summary>Represents the JSON data format.</summary>
    /// <remarks>
    ///     This format is used for serializing and deserializing data in the JSON (JavaScript Object
    ///     Notation) format, a lightweight data interchange format that is easy for humans to read and
    ///     write and easy for machines to parse and generate.
    /// </remarks>
    Json
}