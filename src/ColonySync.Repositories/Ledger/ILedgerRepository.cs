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

using System.Collections.Generic;
using System.Threading.Tasks;
using ColonySync.Common;

namespace ColonySync.Repositories.Ledger;

/// <summary>Represents the contract for a repository that manages Ledger entities.</summary>
public interface ILedgerRepository
{
    /// <summary>
    ///     Asynchronously creates a new ledger entry in the database. If no name is provided, a default name is generated
    ///     based on the current timestamp.
    /// </summary>
    /// <param name="name">The name of the ledger. If null or empty, a default name will be generated.</param>
    /// <returns>
    ///     A task representing the asynchronous operation. The task result contains the created
    ///     <see cref="LedgerEntity" /> if successful, or null if the creation fails.
    /// </returns>
    Task<Result<LedgerEntity>> CreateAsync(string? name = null);

    /// <summary>Retrieves a ledger entity using its unique identifier.</summary>
    /// <param name="id">The unique identifier of the ledger to retrieve.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains the ledger entity if found, or
    ///     null if no ledger with the specified identifier exists.
    /// </returns>
    Task<Result<LedgerEntity>> GetLedgerByIdAsync(string id);

    /// <summary>Asynchronously retrieves a ledger by its name from the database.</summary>
    /// <param name="name">The name of the ledger to retrieve.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a
    ///     <see cref="Result{LedgerEntity}" /> with the retrieved ledger if found, or an error result if no ledger is found or
    ///     the operation fails.
    /// </returns>
    Task<Result<LedgerEntity>> GetLedgerByNameAsync(string name);

    /// <summary>Asynchronously retrieves a list of all ledgers in the repository.</summary>
    /// <returns>
    ///     A read-only list of <see cref="LedgerEntity" /> representing the ledgers in the repository. Returns an empty
    ///     list if no ledgers are found or if an error occurs.
    /// </returns>
    Task<IReadOnlyList<LedgerEntity>> GetLedgersAsync();

    /// <summary>Renames an existing ledger by updating its name in the database.</summary>
    /// <param name="id">The unique identifier of the ledger to be renamed.</param>
    /// <param name="name">The new name to assign to the ledger.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a boolean value indicating whether
    ///     the rename operation was successful.
    /// </returns>
    Task<Result> RenameAsync(string id, string name);

    /// <summary>
    ///     Updates an existing Ledger entity in the database. The update operation is performed based on the entity's
    ///     unique identifier and its last modified timestamp to ensure data consistency.
    /// </summary>
    /// <param name="entity">
    ///     The <see cref="LedgerEntity" /> object containing the updated information for the ledger. The
    ///     entity must have a valid, non-empty identifier.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> representing the asynchronous operation. The task result is a boolean: -
    ///     <c>true</c> if the ledger was successfully updated. - <c>false</c> if the update operation failed or no ledger with
    ///     the specified identifier was found.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if the <paramref name="entity" /> has an invalid or empty identifier.</exception>
    Task<Result> UpdateAsync(LedgerEntity entity);

    /// <summary>Deletes a ledger record from the database by its identifier.</summary>
    /// <param name="id">The unique identifier of the ledger to be deleted.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a boolean value indicating whether
    ///     the operation was successful.
    /// </returns>
    Task<Result> DeleteAsync(string id);
}