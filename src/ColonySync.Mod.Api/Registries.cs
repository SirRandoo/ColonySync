// MIT License
// 
// Copyright (c) 2024 sirrandoo
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

using ColonySync.Common.Interfaces;
using ColonySync.Common.Registries;

namespace ColonySync.Mod.Api;

// FIXME: Currently most registries are empty synchronised registries.
// FIXME: Most registrants currently use an interface without any implementations of said interface.
// TODO: Registries should be initialized at start up, preferably in a way that doesn't block the main thread for extended periods of time.

/// <summary>The Registries class holds static registries for different types of entities involved in the system.</summary>
/// <remarks>The class contains synchronization mechanisms for handling the concurrent manipulation of registry items.</remarks>
public static class Registries
{
    /// <summary>ItemRegistry is a synchronized registry that holds instances of IProduct.</summary>
    /// <remarks>This registry provides thread-safe operations for adding and removing products from the registry.</remarks>
    public static readonly IRegistry<IProduct> ItemRegistry = new SynchronisedRegistry<IProduct>();

    /// <summary>
    ///     A static registry within the <see cref="Registries" /> class dedicated to managing traits. Utilizes
    ///     synchronization mechanisms to ensure thread-safe operations.
    /// </summary>
    /// <remarks>This registry is used to register and unregister objects implementing the <see cref="IProduct" /> interface.</remarks>
    public static readonly IRegistry<IProduct> TraitRegistry = new SynchronisedRegistry<IProduct>();

    /// <summary>Represents a synchronized registry specifically for managing pawn objects within the system.</summary>
    /// <remarks>
    ///     This registry ensures thread-safe operations for adding and removing pawn entities, providing a consistent and
    ///     concurrent-safe way to manage these items.
    /// </remarks>
    public static readonly IRegistry<IProduct> PawnRegistry = new SynchronisedRegistry<IProduct>();

    /// <summary>Represents a static synchronized registry specifically designed for events in the system.</summary>
    /// <remarks>This registry ensures thread-safe operations for registering and unregistering event-related entities.</remarks>
    public static readonly IRegistry<IProduct> EventRegistry = new SynchronisedRegistry<IProduct>();

    /// <summary>LedgerRegistry is a mutable registry holding instances of objects implementing the ILedger interface.</summary>
    /// <remarks>
    ///     This registry allows for the dynamic registration and unregistration of ledger objects throughout the
    ///     application lifecycle.
    /// </remarks>
    public static readonly IRegistry<ILedger> LedgerRegistry = MutableRegistry<ILedger>.CreateInstance([]);

    /// <summary>
    ///     Represents a read-only registry of platform objects. This registry is an instance of the
    ///     <see cref="FrozenRegistry{T}" /> class and is used to store and manage instances of <see cref="IPlatform" />.
    /// </summary>
    public static readonly IReadOnlyRegistry<IPlatform> PlatformRegistry = FrozenRegistry<IPlatform>.CreateInstance([]);
}