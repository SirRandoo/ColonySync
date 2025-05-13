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

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ColonySync.Repositories.Ledger;

/// <summary>
///     Provides an implementation of <see cref="ILedgerCacheService" /> that manages ledger data using a caching
///     mechanism.
/// </summary>
/// <remarks>
///     The service maintains a thread-safe in-memory cache of ledger data to optimize read and retrieval operations.
///     Ledger data is initially loaded from a repository and cached for subsequent access during the application's
///     lifecycle.
/// </remarks>
public sealed class LedgerCacheService(ILedgerRepository repository) : ILedgerCacheService
{
    private readonly ConcurrentDictionary<string, LedgerEntity> _ledgerCache = new();

    /// <inheritdoc />
    public async Task InitializeAsync(CancellationToken token)
    {
        var ledgers = await repository.GetLedgersAsync();

        foreach (var ledgerEntity in ledgers) _ledgerCache.TryAdd(ledgerEntity.Id, ledgerEntity);
    }

    /// <inheritdoc />
    public IReadOnlyCollection<LedgerEntity> GetAllLedgers()
    {
        return _ledgerCache.Values.ToList();
    }

    /// <inheritdoc />
    public bool TryGetLedgerById(string id, [NotNullWhen(true)] out LedgerEntity? ledger)
    {
        return _ledgerCache.TryGetValue(id, out ledger);
    }
}