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
using System.Threading.Tasks;
using ColonySync.Common.Interfaces;

namespace ColonySync.Mod.Shared.Ledgers;

/// <summary>Defines a contract for a service that manages ledger operations.</summary>
public interface ILedgerService
{
    /// <summary>Asynchronously retrieves all ledgers.</summary>
    Task<List<ILedger>> GetAllLedgersAsync();

    /// <summary>Asynchronously retrieves a ledger by its unique identifier.</summary>
    /// <param name="id">The unique identifier of the ledger to retrieve.</param>
    /// <returns>
    ///     A task representing the asynchronous operation, which wraps the ledger if found;
    ///     otherwise, null.
    /// </returns>
    Task<ILedger?> GetLedgerByIdAsync(int id);

    /// <summary>Asynchronously adds a new ledger to the system.</summary>
    /// <param name="ledger">The ledger to be added.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddLedgerAsync(ILedger ledger);

    /// <summary>Asynchronously updates an existing ledger.</summary>
    /// <param name="ledger">The ledger object containing the updated values.</param>
    /// <returns>A task representing the asynchronous update operation.</returns>
    Task UpdateLedgerAsync(ILedger ledger);

    /// <summary>Deletes a ledger entry based on the provided identifier asynchronously.</summary>
    /// <param name="id">The identifier of the ledger to be deleted.</param>
    /// <return>A task that represents the asynchronous delete operation.</return>
    Task DeleteLedgerAsync(int id);
}

public sealed class LedgerService : ILedgerService
{
    /// <inheritdoc />
    public Task<List<ILedger>> GetAllLedgersAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<ILedger?> GetLedgerByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task AddLedgerAsync(ILedger ledger)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task UpdateLedgerAsync(ILedger ledger)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task DeleteLedgerAsync(int id)
    {
        throw new NotImplementedException();
    }
}
