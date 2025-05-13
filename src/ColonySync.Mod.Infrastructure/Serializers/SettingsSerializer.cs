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
using ColonySync.Mod.Shared.Serializers;
using HarmonyLib;
using JetBrains.Annotations;
using NLog;

namespace ColonySync.Mod.Infrastructure.Serializers;

/// <summary>
///     The <c>SettingsSerializer</c> class is responsible for managing the serialization and deserialization of
///     settings for applications or modules. It utilizes a file-based approach to load and save settings using a TOML
///     format serializer.
/// </summary>
/// <remarks>
///     This class implements the <c>ISettingsSerializer</c> interface and is sealed, which means it cannot be
///     inherited. It is designed to work with generic types, allowing for versatility with different types of settings
///     objects.
/// </remarks>
/// <param name="logger">
///     An instance of <c>ILogger</c> used for logging trace, warning, and error messages during the
///     serialization and deserialization process.
/// </param>
/// <param name="serializer">
///     An instance of <c>ITomlDataSerializer</c> used to perform the data serialization and
///     deserialization operations.
/// </param>
[PublicAPI]
internal sealed class SettingsSerializer(ILogger logger, ITomlDataSerializer serializer) : ISettingsSerializer
{
    /// <inheritdoc />
    public bool TryLoadSettings<T>(string path, [NotNullWhen(true)] out T? settings)
    {
        if (!File.Exists(path))
        {
            logger.Trace(message: "File {Path} does not exist :: Failing early...", path);

            settings = default;

            return false;
        }

        using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            var result = serializer.Deserialize<T?>(stream);

            if (result == null)
            {
                logger.Warn(
                    message: "Could not load settings from path {Path} :: Continuing may result in your settings being lost",
                    path
                );

                settings = default;

                return false;
            }

            logger.Trace(message: "Successfully loaded settings from path {Path}", path);
            settings = result;

            return true;
        }
    }

    /// <inheritdoc />
    public T GenerateDefaultSettings<T>()
    {
        logger.Debug(
            message: "Generating default settings for type {QualifiedName} (constructor invocation)",
            typeof(T).FullDescription()
        );

        return (T)Activator.CreateInstance(typeof(T), []);
    }

    /// <inheritdoc />
    public bool TrySaveSettings<T>(string path, [DisallowNull] T settings)
    {
        using (var stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
        {
            try
            {
                serializer.Serialize(stream, settings);

                return true;
            }
            catch (Exception e)
            {
                logger.Error(e, message: "Could not save settings to file {Path}", path);

                return false;
            }
        }
    }
}