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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ColonySync.Common;

namespace ColonySync.Repositories.ItemProduct;

/// <summary>
///     Represents a unique identifier for an item, consisting of a string identifier and a module identifier.
/// </summary>
/// <param name="Id">The unique string identifier for the item.</param>
/// <param name="ModId">The identifier for the module to which the item belongs.</param>
public readonly record struct ItemId(string Id, string ModId);

/// <summary>
///     Represents the repository interface for managing item products, providing operations
///     to retrieve, create, update, rename, and delete item product entities.
/// </summary>
public interface IItemProductRepository
{
    /// <summary>
    ///     Retrieves all item product entities asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A Result containing a read-only list of ItemProductEntity objects if successful; otherwise, an error result.</returns>
    Task<Result<IReadOnlyList<ItemProductEntity>>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a specific item product entity based on the provided item ID.
    /// </summary>
    /// <param name="id">The unique identifier of the item product to retrieve.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    ///     A result containing the requested item product entity if successful, or an appropriate error message if the
    ///     operation fails.
    /// </returns>
    Task<Result<ItemProductEntity>> GetAsync(ItemId id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates a new item product in the repository.
    /// </summary>
    /// <param name="product">The item product entity to create.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>
    ///     A <see cref="Result" /> indicating the success or failure of the operation.
    /// </returns>
    Task<Result> CreateAsync(ItemProductEntity product, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Renames an item identified by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the item to rename.</param>
    /// <param name="name">The new name to assign to the item.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Result" /> indicating the success or failure of the operation.</returns>
    Task<Result> RenameAsync(ItemId id, string name, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates an existing item product in the repository.
    /// </summary>
    /// <param name="product">The item product entity containing updated information.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the request, if necessary.</param>
    /// <returns>A <see cref="Result" /> indicating the success or failure of the update operation.</returns>
    Task<Result> UpdateAsync(ItemProductEntity product, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes an item identified by the specified <paramref name="id" />.
    /// </summary>
    /// <param name="id">The unique identifier of the item to delete.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Result" /> indicating the success or failure of the operation.</returns>
    Task<Result> DeleteAsync(ItemId id, CancellationToken cancellationToken = default);
}

[Flags]
public enum ItemProductProperties
{
    None = 0,
    Apparel = 1,
    MeleeWeapon = 2,
    RangedWeapon = 4,
    Weapon = MeleeWeapon | RangedWeapon,
    Material = 8
}
