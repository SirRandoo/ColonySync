// MIT License
// 
// Copyright (c) 2025 sirrandoo
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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ColonySync.Mod.Shared.Serializers;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace ColonySync.Mod.Infrastructure.Serializers;

/// <summary>
///     Provides functionality to serialize and deserialize data using System.Text.Json's
///     <see cref="JsonSerializer" />.
/// </summary>
/// <param name="logger">
///     An instance of <see cref="ILogger" /> utilized for logging operations during serialization and
///     deserialization.
/// </param>
/// <param name="serializerOptions">Optional settings that control the behavior of the <see cref="JsonSerializer" />.</param>
[PublicAPI]
public sealed class JsonDataSerializer(ILogger logger, JsonSerializerOptions? serializerOptions = null)
    : IJsonDataSerializer
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    /// <inheritdoc />
    public T? Deserialize<T>(Stream stream)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(stream)!;
        }
        catch (Exception e)
        {
            logger.LogError(e, message: "Error deserializing json object from stream");

            return default;
        }
    }

    /// <inheritdoc />
    public void Serialize<T>(Stream stream, [DisallowNull] T data)
    {
        try
        {
            JsonSerializer.Serialize(stream, data, serializerOptions);
        }
        catch (Exception e)
        {
            logger.LogError(e, message: "Error serializing json object into stream");
        }
    }

    /// <inheritdoc />
    public async Task<T?> DeserializeAsync<T>(Stream stream)
    {
        try
        {
            return (await JsonSerializer.DeserializeAsync<T>(stream))!;
        }
        catch (Exception e)
        {
            logger.LogError(e, message: "Error deserializing json object from stream");

            return default;
        }
    }

    /// <inheritdoc />
    public async Task SerializeAsync<T>(Stream stream, [DisallowNull] T data)
    {
        try
        {
            await JsonSerializer.SerializeAsync(stream, data);
        }
        catch (Exception e)
        {
            logger.LogError(e, message: "Error serializing json object into stream");
        }
    }
}
