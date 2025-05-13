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
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace ColonySync.Repositories.Ledger;

/// <summary>Defines the contract for a service that manages and provides cached ledger data.</summary>
public interface ILedgerCacheService
{
    /// <summary>Asynchronously initializes the ledger cache service, preparing it for use.</summary>
    /// <param name="token">A cancellation token that can be used to cancel the operation before completion.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task InitializeAsync(CancellationToken token);

    /// <summary>Retrieves all ledger entities.</summary>
    /// <returns>A read-only collection of ledger entities.</returns>
    IReadOnlyCollection<LedgerEntity> GetAllLedgers();

    /// <summary>Attempts to retrieve a ledger entity by its unique identifier.</summary>
    /// <param name="id">The unique identifier of the ledger to retrieve.</param>
    /// <param name="ledger">
    ///     When this method returns, contains the ledger entity associated with the specified identifier, if
    ///     the identifier was found; otherwise, <c>null</c>.
    /// </param>
    /// <returns><c>true</c> if a ledger entity with the specified identifier is found; otherwise, <c>false</c>.</returns>
    bool TryGetLedgerById(string id, [NotNullWhen(true)] out LedgerEntity? ledger);
}