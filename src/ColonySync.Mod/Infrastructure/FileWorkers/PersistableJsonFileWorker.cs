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
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using ColonySync.Mod.Domain.FileWorkers;
using ColonySync.Mod.Domain.Serializers;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace ColonySync.Mod.Infrastructure.FileWorkers;

/// <summary>A class used to perform file operations on data types within the mod.</summary>
/// <param name="dataSerializer">The data serializer to used to transform data objects.</param>
/// <remarks>
///     The purpose of this class is to perform file operations within the mod. These operations are done in a
///     specific way as to ensure any errors that occur during a file operation will subsequently lock the file from
///     further modifications from the same file worker. These locks only apply to the current instance of the file worker,
///     and do not lock the file for other file workers, external programs, or the operating system.
/// </remarks>
[PublicAPI]
public sealed class PersistableJsonFileWorker(
    ILogger<PersistableJsonFileWorker> logger,
    IJsonDataSerializer dataSerializer
)
    : IPersistableJsonFileWorker
{
    private readonly ConcurrentDictionary<string, object> _blockedFiles = [];

    /// <summary>Returns whether a given path on disk is blocked from being written to.</summary>
    /// <param name="path">The path being queried about.</param>
    /// <remarks>
    ///     When a file worker encounters a problem loading data from disk, the file worker will forbid itself from
    ///     writing data to the file on disk to prevent data loss.
    /// </remarks>
    [PublicAPI]
    public bool IsWritingBlocked(string path)
    {
        return _blockedFiles.ContainsKey(Path.GetFullPath(path));
    }

    /// <summary>Blocks a given path from being written to on disk by the file worker.</summary>
    /// <param name="path">The path being blocked.</param>
    /// <returns>Whether the path was blocked.</returns>
    /// <remarks>
    ///     When a file worker encounters a problems loading data from disk, the file worker will forbid itself from
    ///     writing data to a file on disk to prevent data loss.
    /// </remarks>
    [PublicAPI]
    public bool BlockWriting(string path)
    {
        return _blockedFiles.TryAdd(Path.GetFullPath(path), new object());
    }

    /// <summary>Loads and deserializes the contents of the file at the given path.</summary>
    /// <param name="path">The path to a file on disk to load data from.</param>
    [PublicAPI]
    public T? Read<T>(string path)
    {
        if (!File.Exists(path))
        {
            logger.LogWarning(message: "File not found: {Path}", path);

            return default;
        }

        try
        {
            using (var stream = new FileStream(
                       path, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096,
                       FileOptions.SequentialScan
                   ))
            {
                return dataSerializer.Deserialize<T>(stream);
            }
        }
        catch (Exception e)
        {
            logger.LogError(
                e, message: "Could not load file from disk @ {FilePath}; further save attempts will be blocked", path
            );

            BlockWriting(path);

            return default;
        }
    }

    /// <summary>Loads and deserializes the contents of the file at the given path asynchronously.</summary>
    /// <param name="path">The path to a file on disk to load data from.</param>
    [PublicAPI]
    public async Task<T?> ReadAsync<T>(string path)
    {
        if (!File.Exists(path))
        {
            logger.LogWarning(message: "File not found: {Path}", path);

            return default;
        }

        try
        {
            using (var stream = new FileStream(
                       path, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096,
                       FileOptions.SequentialScan | FileOptions.Asynchronous
                   ))
            {
                return await dataSerializer.DeserializeAsync<T>(stream);
            }
        }
        catch (Exception e)
        {
            logger.LogError(
                e, message: "Could not load file from disk @ {FilePath}; further save attempts will be blocked", path
            );

            BlockWriting(path);

            return default;
        }
    }

    /// <summary>Atomically saves the data provided into the file at the given path.</summary>
    /// <param name="path">The path to a file on disk to save data to.</param>
    /// <param name="value">The data to save to disk.</param>
    [PublicAPI]
    public void Write<T>(string path, [DisallowNull] T value)
    {
        if (IsWritingBlocked(path))
        {
            logger.LogWarning(message: "Save attempts are blocked on the file {Path}", path);

            return;
        }

        var tempFileResult = GetTemporaryFile(path);

        if (string.IsNullOrEmpty(tempFileResult)) return;

        try
        {
            using (var stream = new FileStream(
                       tempFileResult, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, bufferSize: 4096,
                       FileOptions.SequentialScan
                   ))
            {
                dataSerializer.Serialize(stream, value);
            }

            ReplaceFile(tempFileResult!, path);
        }
        catch (Exception e)
        {
            logger.LogError(e, message: "Could not save file to disk @ {FilePath}", path);
        }
    }

    /// <summary>Atomically saves the data provided into the file at the given path asynchronously.</summary>
    /// <param name="path">The path to a file on disk to save data to.</param>
    /// <param name="value">The data to save to disk.</param>
    [PublicAPI]
    public async Task WriteAsync<T>(string path, [DisallowNull] T value)
    {
        if (IsWritingBlocked(path))
        {
            logger.LogWarning(message: "Save attempts are blocked on the file {Path}", path);

            return;
        }

        var tempFileResult = GetTemporaryFile(path);

        if (string.IsNullOrEmpty(tempFileResult)) return;

        try
        {
            using (var stream = new FileStream(
                       tempFileResult,
                       FileMode.OpenOrCreate,
                       FileAccess.Write,
                       FileShare.Read,
                       bufferSize: 4096,
                       FileOptions.SequentialScan | FileOptions.Asynchronous
                   ))
            {
                await dataSerializer.SerializeAsync(stream, value);
            }

            ReplaceFile(tempFileResult!, path);
        }
        catch (Exception e)
        {
            logger.LogError(e, message: "Could not save file to disk @ {FilePath}", path);
        }
    }

    private string? GetTemporaryFile(string originalFilePath)
    {
        var directory = Path.GetDirectoryName(originalFilePath);

        if (string.IsNullOrEmpty(directory))
        {
            logger.LogWarning(message: "The path specified doesn't reside in a directory ({Path})", originalFilePath);

            return null;
        }

        var tempFileName = Path.GetRandomFileName();

        return Path.Combine(directory, tempFileName);
    }

    private void ReplaceFile(string sourceFile, string destinationFile)
    {
        if (!File.Exists(destinationFile))
        {
            logger.LogError(message: "The file at {Path} does not exist", destinationFile);

            return;
        }

        var fileExtension = Path.GetExtension(sourceFile);
        var backupFilePath = Path.ChangeExtension(sourceFile, $".bck.{fileExtension}");

        try
        {
            File.Replace(destinationFile, sourceFile, backupFilePath);
        }
        catch (Exception e)
        {
            logger.LogError(
                e, message: "Could not replace file {Target} with {Replacement}", sourceFile, destinationFile
            );
        }
    }
}
