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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Verse;

namespace ColonySync.Mod.Bootloader;

/// <summary>Provides methods for loading and managing mod assemblies in a given context.</summary>
[SuppressMessage(category: "ReSharper", checkId: "BuiltInTypeReferenceStyle")]
[SuppressMessage(category: "ReSharper", checkId: "BuiltInTypeReferenceStyleForMemberAccess")]
public static class BootModLoader
{
    /// <summary>
    ///     A dictionary that keeps track of running mod classes, mapping their Type to their
    ///     corresponding Mod instance.
    /// </summary>
    private static readonly Dictionary<Type, Verse.Mod> RunningModClassesField =
        AccessTools.StaticFieldRefAccess<Dictionary<Type, Verse.Mod>>("Verse.LoadedModManager:runningModClasses");

    /// <summary>Loads an assembly, and its PDB file if it exists.</summary>
    /// <param name="pack">The mod data being associated with the assembly.</param>
    /// <param name="path">The path to the assembly being loaded.</param>
    /// <returns>The assembly that was loaded from the provided path.</returns>
    public static Assembly? LoadAssembly(ModContentPack pack, string path)
    {
        if (!File.Exists(path))
        {
            Log.Error($"[ColonySync.Bootstrapper] :: Could not load non-existent assembly at path {path} ; Aborting...");

            return null;
        }

        var pdbPath = Path.ChangeExtension(path, extension: ".pdb");
        var content = File.ReadAllBytes(path);
        var pdbContent = File.Exists(pdbPath) ? File.ReadAllBytes(pdbPath) : [];

        Assembly assembly;

        try
        {
            assembly = AppDomain.CurrentDomain.Load(content, pdbContent);
        }
        catch (Exception e)
        {
            Log.Error($"Could not load assembly @ {path}" + e);

            return null;
        }

        pack.assemblies.loadedAssemblies.Add(assembly);

        return assembly;
    }

    /// <summary>
    ///     Runs the static constructors of all types annotated with
    ///     <see cref="StaticConstructorOnStartup" /> in an assembly.
    /// </summary>
    /// <param name="assembly">
    ///     An assembly with types annotated with
    ///     <see cref="StaticConstructorOnStartup" />.
    /// </param>
    public static void RunStaticConstructors(Assembly assembly)
    {
        try
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.TryGetAttribute<StaticConstructorOnStartup>() == null) continue;

                RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            }
        }
        catch (ReflectionTypeLoadException e)
        {
            Log.Error($"Encountered an error getting types in assembly {assembly.GetName().Name}" + e);

            foreach (var exception in e.LoaderExceptions) Log.Error(exception.ToString());
        }
    }

    /// <summary>
    ///     Instantiates <see cref="Mod" /> classes, and adds them to an internal field in RimWorld's
    ///     <see cref="LoadedModManager" /> class.
    /// </summary>
    /// <param name="mod">The mod the assembly belongs to.</param>
    /// <param name="assembly">The assembly being indexed for <see cref="Mod" /> classes.</param>
    public static void InstantiateModClasses(ModContentPack mod, Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
            try
            {
                if (typeof(Verse.Mod).IsAssignableFrom(type) &&
                    Activator.CreateInstance(type, mod) is Verse.Mod instance)
                    RunningModClassesField.Add(type, instance);
            }
            catch (Exception e)
            {
                Log.Error($"Could not instantiate {type.FullDescription()}" + e);
            }
    }
}
