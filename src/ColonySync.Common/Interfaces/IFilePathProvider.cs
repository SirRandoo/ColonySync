// MIT License
//
// Copyright (c) 2024 SirRandoo
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

namespace ColonySync.Common.Interfaces;

/// <summary>
///     Provides a contract for retrieving file paths used by the mod in a structured and
///     organized manner.
/// </summary>
public interface IFilePathProvider
{
    /// <summary>Retrieves the full path to a data file within the application's data directory.</summary>
    /// <param name="fileName">The name of the data file, including its extension (e.g., "data.db").</param>
    /// <returns>A string representing the absolute path to the specified data file.</returns>
    string GetDataFile(string fileName);

    /// <summary>Retrieves the full path to a file within the root directory of the mod.</summary>
    /// <param name="fileName">
    ///     The name of the file within the root directory, including its extension
    ///     (e.g., "config.json").
    /// </param>
    /// <returns>A string representing the absolute path to the specified file in the root directory.</returns>
    string GetRootFile(string fileName);

    /// <summary>Retrieves the full path to a settings file within the mod's settings directory.</summary>
    /// <param name="fileName">
    ///     The name of the settings file, including its extension (e.g.,
    ///     "runtime-flags.txt").
    /// </param>
    /// <returns>A string representing the absolute path to the specified settings file.</returns>
    string GetSettingsFile(string fileName);
}