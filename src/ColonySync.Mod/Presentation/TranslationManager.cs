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
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Verse;

namespace ColonySync.Mod.Presentation;

/// <summary>Manages the registration and unregistration of translation fields within types.</summary>
[PublicAPI]
public static class TranslationManager
{
    private static readonly List<TranslationIndex> TranslationIndices = [];
    private static readonly Dictionary<FieldInfo, TranslationIndex> TranslationIndicesKeyed = [];
    private static readonly List<TranslationListener> TranslationListeners = [];
    private static readonly Dictionary<MethodInfo, TranslationListener> TranslationListenersKeyed = [];

    /// <summary>
    ///     Registers any fields marked with <see cref="TranslationAttribute" /> found within the
    ///     type.
    /// </summary>
    /// <param name="type">The type to index for translation fields.</param>
    public static void Register(Type type)
    {
        var @namespace = type.GetCustomAttribute<TranslationNamespaceAttribute>();

        RegisterFields(type, @namespace);
        RegisterListeners(type);
    }

    /// <summary>
    ///     Registers fields marked with <see cref="TranslationAttribute" /> found within the
    ///     specified type.
    /// </summary>
    /// <param name="type">The type to search for translation fields.</param>
    /// <param name="namespace">The namespace to use for translation keys.</param>
    private static void RegisterFields(Type type, TranslationNamespaceAttribute @namespace)
    {
        foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public))
        {
            var attribute = field.GetCustomAttribute<TranslationAttribute>();

            if (attribute == null) continue;

            var reference = AccessTools.StaticFieldRefAccess<string>(field);
            var fullKey = @namespace != null ? $"{@namespace.Namespace}.{attribute.Key}" : attribute.Key;

            var translationIndex = new TranslationIndex(field, reference, fullKey, attribute.Color);

            TranslationIndices.Add(translationIndex);
            TranslationIndicesKeyed.Add(field, translationIndex);

            if (string.IsNullOrEmpty(reference())) RecacheTranslation(translationIndex);
        }
    }

    /// <summary>
    ///     Registers listeners by examining methods within the given type that are decorated with the
    ///     <see cref="TranslationRecacheListenerAttribute" />.
    /// </summary>
    /// <param name="type">The type containing the methods to be indexed for translation listeners.</param>
    private static void RegisterListeners(Type type)
    {
        foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public))
        {
            var attribute = method.GetCustomAttribute<TranslationRecacheListenerAttribute>();

            if (attribute == null) continue;

            var @delegate = AccessTools.MethodDelegate<ListenerDelegate>(method);
            var listener = new TranslationListener(method, @delegate);

            TranslationListeners.Add(listener);
            TranslationListenersKeyed.Add(method, listener);

            @delegate();
        }
    }

    /// <summary>Removes any previously registered fields within the given type.</summary>
    /// <param name="type">The type to unregister translation fields from.</param>
    public static void Unregister(Type type)
    {
        for (var index = TranslationIndices.Count - 1; index >= 0; index--)
        {
            var field = TranslationIndices[index];

            if (field.Field.DeclaringType != type) continue;

            TranslationIndices.Remove(field);
            TranslationIndicesKeyed.Remove(field.Field);
        }

        for (var index = 0; index < TranslationListeners.Count; index++)
        {
            var listener = TranslationListeners[index];

            if (listener.Method.DeclaringType != type) continue;

            TranslationListeners.Remove(listener);
            TranslationListenersKeyed.Remove(listener.Method);
        }
    }

    /// <summary>
    ///     Updates the translation for the given translation index using the current language
    ///     settings.
    /// </summary>
    /// <param name="index">The translation index containing the field and metadata to update.</param>
    private static void RecacheTranslation(TranslationIndex index)
    {
        var key = index.Key;

        if (key.CanTranslate()) key = key.TranslateSimple();

        if (!string.IsNullOrEmpty(index.Color))
            key = index.Color!.StartsWith("#")
                ? $"""<color="{index.Color}">{key}</color>"""
                : $"""<color="#{index.Color}">{key}</color>""";

        ref var indexRef = ref index.Ref();
        indexRef = key;
    }

    /// <summary>Implements a Harmony patch to facilitate language selection in the application.</summary>
    [PublicAPI]
    [HarmonyPatch]
    private static class SelectLanguagePatch
    {
        /// <summary>Represents the method used for selecting a language within the language database.</summary>
        private static readonly MethodBase SelectLanguageMethod =
            AccessTools.Method(typeof(LanguageDatabase), nameof(LanguageDatabase.SelectLanguage));

        /// <summary>Provides methods to be targeted for Harmony patches.</summary>
        /// <returns>A sequence of methods to be patched by Harmony.</returns>
        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return SelectLanguageMethod;
        }

        /// <summary>Executes after the SelectLanguage method is called to refresh translations.</summary>
        private static void Postfix()
        {
            RecacheTranslations();
        }

        /// <summary>
        ///     Recaches all registered translations, ensuring that the translated values are up-to-date.
        ///     This involves updating the translation keys and invoking listener delegates.
        /// </summary>
        private static void RecacheTranslations()
        {
            for (var i = 0; i < TranslationIndices.Count; i++) RecacheTranslation(TranslationIndices[i]);

            for (var i = 0; i < TranslationListeners.Count; i++) TranslationListeners[i].Delegate();
        }
    }

    /// <summary>Delegate for methods that are invoked when the translation cache is refreshed.</summary>
    private delegate Delegate ListenerDelegate();

    /// <summary>
    ///     Represents an index for a translation field which includes metadata such as the field
    ///     information, reference string, key, and color.
    /// </summary>
    private sealed record TranslationIndex(
        FieldInfo Field,
        AccessTools.FieldRef<string> Ref,
        string Key,
        string? Color);

    /// <summary>
    ///     Represents a listener for translation recache events, holding a method and its
    ///     corresponding delegate. Used primarily within the translation management system.
    /// </summary>
    private sealed record TranslationListener(MethodInfo Method, ListenerDelegate Delegate);
}

/// <summary>Provides methods to apply or remove runtime patches to existing code.</summary>
[UsedImplicitly]
[StaticConstructorOnStartup]
internal static class Patcher
{
    /// <summary>An instance of the Harmony class used for patching methods at runtime.</summary>
    private static readonly Harmony HarmonyInstance = new("com.sirrandoo.ux");

    /// <summary>Static class responsible for applying all Harmony patches on assembly load.</summary>
    static Patcher()
    {
        HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
    }
}