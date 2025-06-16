namespace ColonySync.Mod.Domain.Serializers;

/// <summary>
///     Defines a serializer interface for serializing and deserializing data streams to and from
///     objects in TOML format. Suitable for Dependency Injection to abstract the TOML serialization
///     format.
/// </summary>
public interface ITomlDataSerializer : IDataSerializer
{
}