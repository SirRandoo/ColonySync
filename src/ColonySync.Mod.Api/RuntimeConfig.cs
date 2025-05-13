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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ColonySync.Mod.Api.Attributes;

namespace ColonySync.Mod.Api;

/// <summary>Represents a set of runtime-configurable internal settings for the mod.</summary>
public static class RuntimeConfig
{
    /// <summary>
    ///     Represents a set of internal settings flags that may be set or queried at runtime to
    ///     modify the behavior of certain systems within the mod.
    /// </summary>
    private static readonly ConcurrentDictionary<string, object> Flags = [];

    /// <summary>Retrieves a read-only list of all current runtime flags set within the config.</summary>
    public static IReadOnlyList<string> AllFlags => Flags.Keys.ToList();

    /// <summary>
    ///     Event that is fired whenever a runtime flag is added, removed, or toggled within the
    ///     configuration. Subscribers can listen to this event to respond to changes in the runtime flags.
    /// </summary>
    public static event EventHandler<RuntimeFlagChangedEventArgs>? RuntimeFlagChanged;

    /// <summary>Sets a flag within the config.</summary>
    /// <param name="flag">The flag to set.</param>
    /// <returns>Whether the flag was added to the config.</returns>
    public static bool SetFlag(string flag)
    {
        var added = Flags.TryAdd(flag, new object());

        if (added)
            OnRuntimeFlagChanged(new RuntimeFlagChangedEventArgs
            {
                Flag = flag, Active = true
            });

        return added;
    }

    /// <summary>Returns whether a flag is currently set within the config.</summary>
    /// <param name="flag">The flag to check for.</param>
    /// <returns>True if the flag is set; otherwise, false.</returns>
    public static bool HasFlag(string flag)
    {
        return Flags.ContainsKey(flag);
    }

    /// <summary>Unsets a flag within the config.</summary>
    /// <param name="flag">The flag to unset.</param>
    /// <returns>Whether the flag was removed from the config.</returns>
    public static bool UnsetFlag(string flag)
    {
        var removed = Flags.TryRemove(flag, out _);

        if (removed)
            OnRuntimeFlagChanged(new RuntimeFlagChangedEventArgs
            {
                Flag = flag, Active = false
            });

        return removed;
    }

    /// <summary>Toggles a flag within the config.</summary>
    /// <param name="flag">The flag to toggle.</param>
    /// <returns>Whether the flag was toggled.</returns>
    public static bool ToggleFlag(string flag)
    {
        var changed = Flags.ContainsKey(flag) ? Flags.TryRemove(flag, out _) : Flags.TryAdd(flag, new object());

        if (changed)
            OnRuntimeFlagChanged(new RuntimeFlagChangedEventArgs
            {
                Flag = flag, Active = changed
            });

        return changed;
    }

    /// <summary>Triggers the RuntimeFlagChanged event.</summary>
    /// <param name="e">The event arguments containing details about the flag change.</param>
    private static void OnRuntimeFlagChanged(RuntimeFlagChangedEventArgs e)
    {
        RuntimeFlagChanged?.Invoke(sender: null, e);
    }

    /// <summary>A container class that houses all flags that are supported by the mod.</summary>
    public static class RuntimeFlags
    {
        /// <summary>
        ///     Indicates whether to disable Harmony's stacktrace caching. This flag should be activated
        ///     if you are experiencing issues, as Harmony may remove important information from errors.
        /// </summary>
        [Experimental]
        [Description("Whether to disable Harmony's, the RimWorld mod's, stacktrace caching.")]
        [Description(
            "This flag should be active if you're actively experiencing a problem, since Harmony may remove important information from errors.")]
        public const string DisableHarmonyStacktraceCaching = "dev.harmony.exception.cache.disable";

        /// <summary>
        ///     Represents a runtime flag that, when set, disables the stacktrace enhancing feature in
        ///     Harmony, a modding library for RimWorld. By disabling this feature, the stacktraces will remain
        ///     unchanged, which can be useful for debugging issues where Harmony's enhanced stacktraces might
        ///     obscure important information.
        /// </summary>
        [Experimental]
        [Description("Whether to disable Harmony, the RimWorld mod's, stacktrace enhancing.")]
        [Description("If this flag isn't active, Harmony will adjust errors with patch information.")]
        [Description(
            "This flag should be active if you're actively experiencing a problem, since Harmony may remove important information from errors.")]
        public const string DisableHarmonyStacktraceEnhancing = "dev.harmony.exception.prettifier.disable";
    }
}

/// <summary>
///     Event arguments for runtime configuration flag changes. Provides details about the flag
///     that was changed and its new active state.
/// </summary>
public class RuntimeFlagChangedEventArgs : EventArgs
{
    /// <summary>Indicates whether a specific runtime flag is currently active or inactive.</summary>
    public bool Active { get; init; }

    /// <summary>
    ///     Represents a specific runtime flag that can be set or unset to control various
    ///     configurations or features in the application.
    /// </summary>
    public required string Flag { get; init; }
}
