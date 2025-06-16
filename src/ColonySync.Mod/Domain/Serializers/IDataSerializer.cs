// MIT License
//
// Copyright (c) 2023 SirRandoo
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

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace ColonySync.Mod.Domain.Serializers;

/// <summary>
///     Defines a serializer interface for serializing and deserializing data streams to and from
///     objects. Suitable for Dependency Injection to abstract different serialization formats like
///     JSON, XML, etc.
/// </summary>
public interface IDataSerializer
{
    /// <summary>Deserializes the data from a stream into an object of type <typeparamref name="T" />.</summary>
    /// <typeparam name="T">The type of the object being deserialized.</typeparam>
    /// <param name="stream">The stream containing the serialized data.</param>
    /// <returns>
    ///     The deserialized object of type <typeparamref name="T" />, or the default value of
    ///     <typeparamref name="T" /> if deserialization fails.
    /// </returns>
    T? Deserialize<T>(Stream stream);

    /// <summary>Serializes an object of type <typeparamref name="T" /> to a stream.</summary>
    /// <typeparam name="T">The type of the object being serialized.</typeparam>
    /// <param name="stream">The stream to which the object is serialized.</param>
    /// <param name="data">The object to serialize. Must not be null.</param>
    void Serialize<T>(Stream stream, [DisallowNull] T data);

    /// <summary>
    ///     Asynchronously deserializes the data from a stream into an object of type
    ///     <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The type of the object being deserialized.</typeparam>
    /// <param name="stream">The stream containing the serialized data.</param>
    /// <returns>
    ///     A task representing the asynchronous operation, containing the deserialized object of type
    ///     <typeparamref name="T" />, or the default value if deserialization fails.
    /// </returns>
    Task<T?> DeserializeAsync<T>(Stream stream);

    /// <summary>Asynchronously serializes an object of type <typeparamref name="T" /> to a stream.</summary>
    /// <typeparam name="T">The type of the object being serialized.</typeparam>
    /// <param name="stream">The stream to which the object is serialized.</param>
    /// <param name="data">The object to serialize. Must not be null.</param>
    /// <returns>A task representing the asynchronous serialization operation.</returns>
    Task SerializeAsync<T>(Stream stream, [DisallowNull] T data);
}
