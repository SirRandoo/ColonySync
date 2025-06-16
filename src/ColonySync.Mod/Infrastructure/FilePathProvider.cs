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
using System.IO;
using ColonySync.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Verse;

namespace ColonySync.Mod.Infrastructure;

/// <summary>Provides methods to retrieve file paths for various types of files used by the mod.</summary>
public sealed class FilePathProvider : IFilePathProvider
{
    private readonly string _baseDirectory;
    private readonly string _dataBase;
    private readonly ILogger _logger;
    private readonly string _settingsBase;

    public FilePathProvider(ILogger logger)
    {
        _logger = logger;
        _baseDirectory = GetDirectory(GenFilePaths.SaveDataFolderPath, directory: "ColonySync");
        _settingsBase = GetDirectory(_baseDirectory, directory: "settings");
        _dataBase = GetDirectory(_baseDirectory, directory: "data");
    }

    /// <inheritdoc />
    public string GetDataFile(string fileName)
    {
        return Path.Combine(_dataBase, fileName);
    }

    /// <inheritdoc />
    public string GetRootFile(string fileName)
    {
        return Path.Combine(_baseDirectory, fileName);
    }

    /// <inheritdoc />
    public string GetSettingsFile(string fileName)
    {
        return Path.Combine(_settingsBase, fileName);
    }

    private string GetDirectory(string parent, string directory, bool ensureExists = true)
    {
        var path = Path.Combine(parent, directory);

        if (!ensureExists || Directory.Exists(path)) return path;

        try
        {
            Directory.CreateDirectory(path);
        }
        catch (Exception e)
        {
            _logger.LogError(
                e, message: "{Path} could not be created. Things may not work correctly, or at all", path
            );
        }

        return path;
    }
}