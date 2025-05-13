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
    private static readonly string? NativeExtension = GetNativeExtension();
    private static readonly XmlSerializer Serializer = new(typeof(Corpus));
    private static readonly List<string> SpecialFiles = [];

    static Bootstrap()
    {
        Log.Message($"[ColonySync.Bootstrapper] :: ColonySync is running on the platform '{UnityData.platform}'");

        if (string.IsNullOrEmpty(NativeExtension))
        {
            Log.Error(
                $"[ColonySync.Bootstrapper] :: '{UnityData.platform}' is an unsupported platform; Aborting initialization...");

            return;
        }

        DisableHarmonyStacktraceCaching();
        DisableHarmonyStacktraceEnhancing();

        Application.wantsToQuit += CleanNativeFiles;

        foreach (var mod in LoadedModManager.RunningMods)
        {
            var corpusPath = Path.Combine(mod.RootDir, path2: "Corpus.xml");

            if (!File.Exists(corpusPath)) continue;

            LoadContent(mod, corpusPath);
        }
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
    ///     Deletes all files listed in the SpecialFiles list. Logs any errors encountered during the
    ///     deletion process.
    /// </summary>
    /// <returns>
    ///     Returns true if the operation completes, regardless of success or failure of the
    ///     deletions.
    /// </returns>
    private static bool CleanNativeFiles()
    {
        for (var index = 0; index < SpecialFiles.Count; index++)
        {
            var file = SpecialFiles[index];

            try
            {
                File.Delete(file);
            }
            catch (Exception e)
            {
                Log.Error($"[ColonySync.Bootstrapper] :: Could not clean file @ {file}" + e);
                Log.Warning(
                    $"[ColonySync.Bootstrapper] :: Any mod updates pending to {file} may not go through next relaunch");
            }
        }

        return true;
    }

    /// <summary>
    ///     Loads and deserializes XML content from the specified corpus path, then processes the
    ///     loaded resources for the given mod.
    /// </summary>
    /// <param name="mod">The mod content pack for which the content is being loaded.</param>
    /// <param name="corpusPath">The file path to the XML corpus containing the content to load.</param>
    private static void LoadContent(ModContentPack mod, string corpusPath)
    {
        Corpus? corpus;

        using (var stream = File.Open(corpusPath, FileMode.Open, FileAccess.Read))
        {
            corpus = Serializer.Deserialize(stream) as Corpus;
        }

        if (corpus is null)
        {
            Log.Error(
                $"[ColonySync.Bootstrapper] :: Object within corpus file for {mod.Name} was malformed. Aborting...");

            return;
        }

        LoadResources(mod, corpus);
    }

    /// <summary>Loads the resources specified in the given corpus into the provided mod content pack.</summary>
    /// <param name="mod">The mod content pack where resources are loaded.</param>
    /// <param name="corpus">The corpus containing resources to be loaded.</param>
    private static void LoadResources(ModContentPack mod, Corpus corpus)
    {
        foreach (var bundle in corpus.Resources) LoadResourceBundle(mod, bundle);
    }

    /// <summary>
    ///     Loads a resource bundle for a given mod, handling various resource types such as DLLs,
    ///     assemblies, and standard managed files.
    /// </summary>
    /// <param name="mod">The mod content pack containing the resources.</param>
    /// <param name="bundle">The resource bundle to be loaded.</param>
    private static void LoadResourceBundle(ModContentPack mod, ResourceBundle bundle)
    {
        var path = GetPathFor(mod, bundle);

        if (!Directory.Exists(path))
        {
            Log.Error(
                $"[ColonySync.Bootstrapper] :: The directory {path} doesn't exist, but was specified in {mod.Name}'s corpus. Aborting...");

            return;
        }

        var assemblyCandidates = new List<Assembly>();

        foreach (var resource in bundle.Resources)
        {
            var resourceDir = string.IsNullOrEmpty(resource.Root)
                ? Path.GetFullPath(path)
                : Path.GetFullPath(Path.Combine(path, resource.Root));

            switch (resource.Type)
            {
                case ResourceType.Dll:
                    CopyNativeFile(resource, resourceDir);

                    break;
                case ResourceType.Assembly:
                    var assembly = BootModLoader.LoadAssembly(
                        mod,
                        string.IsNullOrEmpty(resource.Root)
                            ? Path.Combine(path, path2: "Assemblies", $"{resource.Name}.dll")
                            : Path.Combine(path, resource.Root, path3: "Assemblies", $"{resource.Name}.dll")
                    );

                    if (assembly == null) break;

                    assemblyCandidates.Add(assembly);

                    break;
                case ResourceType.NetStandardAssembly:
                    CopyStandardManagedFile(resource, resourceDir);

                    break;
                default:
                    Log.Warning(
                        @$"[ColonySync.Bootstrapper] :: Cannot load resource ""{resource.Name}"" due to an unsupported type of ""{resource.Type.ToStringFast()}""");

                    break;
            }
        }

        if (assemblyCandidates.Count <= 0) return;

        foreach (var assembly in assemblyCandidates)
        {
            try
            {
                BootModLoader.InstantiateModClasses(mod, assembly);
            }
            catch (Exception e)
            {
                Log.Error(
                    $"[ColonySync.Bootstrapper] :: Encountered one or more errors while instantiating mod classes for assembly {assembly.GetName()} from {mod.PackageId}" +
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
                    $"[ColonySync.Bootstrapper] :: Encounter one or more errors while running static constructors for assembly {assembly.GetName()} from {mod.PackageId}" +
                    e
                );
            }
        }
    }

    /// <summary>Copies a native file from the resource directory to the current directory.</summary>
    /// <param name="resource">The resource representing the file to be copied.</param>
    /// <param name="resourceDir">The directory where the resource file is located.</param>
    private static void CopyNativeFile(Resource resource, string resourceDir)
    {
        var fileName = $"{resource.Name}.{NativeExtension}";
        var resourcePath = Path.Combine(resourceDir, $"{resource.Name}.{NativeExtension}");
        var destinationPath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

        if (File.Exists(destinationPath) || !File.Exists(resourcePath) && resource.Optional) return;

        try
        {
            File.Copy(resourcePath, destinationPath);

            SpecialFiles.Add(destinationPath);
        }
        catch (Exception e)
        {
            Log.Error(
                $"[ColonySync.Bootstrapper] :: Could not copy {fileName} to {destinationPath} (from {resourcePath}). Things will not work correctly" +
                e);
        }
    }


    /// <summary>
    ///     Copies a managed assembly file from the specified resource directory to the current
    ///     working directory if it does not already exist.
    /// </summary>
    /// <param name="resource">The resource representing the managed assembly file to copy.</param>
    /// <param name="resourceDir">The directory path where the resource is located.</param>
    private static void CopyStandardManagedFile(Resource resource, string resourceDir)
    {
        var fileName = $"{resource.Name}.dll";
        var resourcePath = Path.Combine(resourceDir, $"{resource.Name}.dll");
        var destinationPath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

        if (File.Exists(destinationPath)) return;

        try
        {
            File.Copy(resourcePath, destinationPath);

            SpecialFiles.Add(destinationPath);
        }
        catch (Exception e)
        {
            Log.Error(
                $"[ColonySync.Bootstrapper] :: Could not copy {fileName} to {destinationPath} (from {resourcePath}). Things will not work correctly" +
                e);
        }
    }

    /// <summary>
    ///     Retrieves the path for the specified mod content pack and resource bundle. The path is
    ///     constructed based on the root directory of the mod and the properties of the resource bundle,
    ///     such as the root and versioning.
    /// </summary>
    /// <param name="mod">The mod content pack for which the path is being retrieved.</param>
    /// <param name="bundle">The resource bundle whose path is being constructed.</param>
    /// <returns>A string representing the path to the specified resource bundle.</returns>
    private static string GetPathFor(ModContentPack mod, ResourceBundle bundle)
    {
        var root = mod.RootDir;

        if (!string.IsNullOrEmpty(bundle.Root)) root = Path.Combine(root, bundle.Root);

        if (!bundle.Versioned) return root;

        var withoutBuild = Path.Combine(root, VersionControl.CurrentVersionString);

        return Directory.Exists(withoutBuild)
            ? withoutBuild
            : Path.Combine(root, VersionControl.CurrentVersionStringWithoutBuild);
    }

    /// <summary>Determines the native file extension for the current runtime platform.</summary>
    /// <returns>
    ///     A string representing the file extension for the current platform (.dll for Windows, .so
    ///     for Linux, .dylib for OSX). If the platform is not recognized, returns null.
    /// </returns>
    private static string? GetNativeExtension()
    {
        return UnityData.platform switch
        {
            RuntimePlatform.WindowsPlayer => ".dll",
            RuntimePlatform.LinuxPlayer => ".so",
            RuntimePlatform.OSXPlayer => ".dylib",
            _ => null
        };
    }
}