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
using System.Threading.Tasks;
using ColonySync.Mod.Shared.Serializers;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using Tomlet;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ColonySync.Mod.Infrastructure.Serializers;

/// <summary>
///     The <c>TomlDataSerializer</c> class provides methods for serializing and deserializing data to and from TOML
///     format.
/// </summary>
/// <remarks>
///     This class implements the <see cref="ITomlDataSerializer" /> interface and provides functionality for both
///     synchronous and asynchronous operations.
/// </remarks>
public sealed class TomlDataSerializer(ILogger logger) : ITomlDataSerializer
{
    /// <inheritdoc />
    public T? Deserialize<T>(Stream stream)
    {
        try
        {
            using (var reader = new StreamReader(stream))
            {
                return TomletMain.To<T>(reader.ReadToEnd());
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, message: "Could not deserialize toml string");

            return default;
        }
    }

    /// <inheritdoc />
    public void Serialize<T>(Stream stream, [DisallowNull] T data)
    {
        try
        {
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(TomletMain.TomlStringFrom(data));
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, message: "Could not serialize {Type} to toml string", typeof(T).FullDescription());
        }
    }

    /// <inheritdoc />
    public async Task<T?> DeserializeAsync<T>(Stream stream)
    {
        try
        {
            using (var reader = new StreamReader(stream))
            {
                return TomletMain.To<T>(await reader.ReadToEndAsync());
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, message: "Could not deserialize toml string");

            return default;
        }
    }

    /// <inheritdoc />
    public async Task SerializeAsync<T>(Stream stream, [DisallowNull] T data)
    {
        try
        {
            using (var writer = new StreamWriter(stream))
            {
                await writer.WriteAsync(TomletMain.TomlStringFrom(data));
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, message: "Could not serialize {Type} to toml string", typeof(T).FullDescription());
        }
    }
}