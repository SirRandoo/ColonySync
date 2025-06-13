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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace ColonySync.Mod.Bootloader;

/// <summary>
///     The Bootstrap class is a static initializer used to set up the initial state and
///     configurations for the application. This class is automatically invoked at the start of the
///     application due to the StaticConstructorOnStartup attribute.
/// </summary>
[StaticConstructorOnStartup]
[SuppressMessage(category: "ReSharper", checkId: "BuiltInTypeReferenceStyleForMemberAccess")]
internal static class Bootstrap
{
    static Bootstrap()
    {
        Log.Message($"[ColonySync.Bootstrapper] :: ColonySync is running on the platform '{UnityData.platform}'");

        DisableHarmonyStacktraceCaching();
        DisableHarmonyStacktraceEnhancing();

        string modDirectory = LoadedModManager.GetMod<BootstrapMod>().Content.RootDir;
        string releasesDirectory = Path.Combine(modDirectory, "Releases");

        AppendNativeDirectoryToLookupPath(releasesDirectory);
        LoadDeferredAssemblies(releasesDirectory);
    }

    private static void AppendNativeDirectoryToLookupPath(string directory)
    {
        if (UnityData.platform is not (RuntimePlatform.WindowsPlayer or RuntimePlatform.OSXPlayer or RuntimePlatform.LinuxPlayer))
        {
            Log.Error("[ColonySync.Bootstrapper] :: Running StreamKit on an unsupported operating system; aborting modifying native resolution path...");

            return;
        }

        string nativesDirectory = Path.Combine(directory, "natives");

        if (nativesDirectory[^1] != Path.PathSeparator) nativesDirectory += Path.PathSeparator;

        switch (UnityData.platform)
        {
            case RuntimePlatform.WindowsPlayer:
                AddNativeDirectoryToPath(nativesDirectory);

                break;
            case RuntimePlatform.OSXPlayer:
                AddNativeDirectoryToFrameworkPath(nativesDirectory);

                break;
            case RuntimePlatform.LinuxPlayer:
                AddNativeDirectoryToLibraryPath(nativesDirectory);

                break;
        }
    }

    private static void AddNativeDirectoryToFrameworkPath(string directory)
    {
        const string frameworkPathKey = "DYLD_FRAMEWORK_PATH";
        string? frameworkVariable = Environment.GetEnvironmentVariable(frameworkPathKey);

        Environment.SetEnvironmentVariable(frameworkPathKey, string.IsNullOrWhiteSpace(frameworkVariable) ? $"{directory}:" : $"{directory}:{frameworkVariable}");
    }

    private static void AddNativeDirectoryToLibraryPath(string directory)
    {
        const string libraryPathKey = "LD_LIBRARY_PATH";
        string? libraryVariable = Environment.GetEnvironmentVariable(libraryPathKey);

        Environment.SetEnvironmentVariable(libraryPathKey, string.IsNullOrWhiteSpace(libraryVariable) ? $"{directory}:" : $"{libraryVariable}:{directory}");
    }

    private static void AddNativeDirectoryToPath(string directory)
    {
        const string pathKey = "PATH";
        string? pathEnvVariable = Environment.GetEnvironmentVariable(pathKey);

        Environment.SetEnvironmentVariable(pathKey, string.IsNullOrWhiteSpace(pathEnvVariable) ? $"{directory};" : $"{pathEnvVariable};{directory}");
    }

    /// <summary>
    ///     Disables the Harmony library's stacktrace caching functionality when in DEBUG mode to
    ///     facilitate easier debugging.
    /// </summary>
    /// <remarks>
    ///     This method sets the internal noStacktraceCaching field in the Harmony library to true,
    ///     preventing stack traces from being cached. This change might be necessary when debugging issues
    ///     related to stack traces in Harmony-patched methods.
    /// </remarks>
    /// <exception cref="Exception">
    ///     Thrown if the Harmony library's noStacktraceCaching field cannot be
    ///     accessed or modified.
    /// </exception>
    [Conditional("DEBUG")]
    private static void DisableHarmonyStacktraceCaching()
    {
        try
        {
            ref var cachingField =
                ref AccessTools.StaticFieldRefAccess<bool>("HarmonyMod.HarmonyMain:noStacktraceCaching");
            cachingField = true;
        }
        catch (Exception e)
        {
            Log.Error("[ColonySync.Bootstrapper] :: Could not toggle Harmony's, the RimWorld mod, stacktrace caching." +
                      e);
        }
    }

    /// <summary>
    ///     Disables the enhancing of stack traces by the Harmony library. This method is a
    ///     conditional method and will only be executed in debug builds. It attempts to disable the stack
    ///     trace enhancing by setting the internal field within Harmony. If the enhancing cannot be
    ///     disabled due to an exception, an error is logged.
    /// </summary>
    [Conditional("DEBUG")]
    private static void DisableHarmonyStacktraceEnhancing()
    {
        try
        {
            ref var prettifierField =
                ref AccessTools.StaticFieldRefAccess<bool>("HarmonyMod.HarmonyMain:noStacktraceEnhancing");
            prettifierField = true;
        }
        catch (Exception e)
        {
            Log.Error("[ColonySync.Bootstrapper] :: Could not toggle Harmony's, the RimWorld mod, stacktrace enhancing" +
                      e);
        }
    }

    /// <summary>
    ///     Loads assemblies meant to be loaded independently of RimWorld load sequence.
    /// </summary>
    /// <param name="directory">The directory to load recursively load assemblies from.</param>
    private static void LoadDeferredAssemblies(string directory)
    {
        List<Assembly> loadedAssemblies = [];
        var modContentPack = LoadedModManager.GetMod<BootstrapMod>().Content;

        foreach (string file in Directory.EnumerateFiles(directory, "*.dll", SearchOption.AllDirectories))
        {
            var assembly = BootModLoader.LoadAssembly(modContentPack, file);

            if (assembly == null)
            {
                Log.Error($"[ColonySync.Bootstrapper] :: Could not load assembly @ {file} -- things will not work properly.");

                continue;
            }

            loadedAssemblies.Add(assembly);
        }

        foreach (var assembly in loadedAssemblies)
        {
            try
            {
                BootModLoader.InstantiateModClasses(modContentPack, assembly);
            }
            catch (Exception e)
            {
                Log.Error(
                    $"[ColonySync.Bootstrapper] :: Encountered one or more errors while instantiating mod classes for assembly {assembly.GetName()} from {modContentPack.PackageId}" +
                    e
                );
            }

            try
            {
                BootModLoader.RunStaticConstructors(assembly);
            }
            catch (Exception e)
            {
                Log.Error(
                    $"[ColonySync.Bootstrapper] :: Encounter one or more errors while running static constructors for assembly {assembly.GetName()} from {modContentPack.PackageId}" +
                    e
                );
            }
        }
    }
}
