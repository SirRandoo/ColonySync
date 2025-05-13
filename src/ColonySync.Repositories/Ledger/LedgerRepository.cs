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
using System.Collections.Immutable;
using System.Data.Common;
using System.Threading.Tasks;
using ColonySync.Common;
using ColonySync.Common.Extensions;
using Dapper;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace ColonySync.Repositories.Ledger;

/// <summary>
///     Provides methods for managing ledger entities, including creation, retrieval, updating, renaming, and
///     deletion.
/// </summary>
[PublicAPI]
public sealed class LedgerRepository(DbProviderFactory factory, ILogger logger) : ILedgerRepository
{
    /// <inheritdoc />
    public async Task<Result<LedgerEntity>> CreateAsync(string? name = null)
    {
        if (string.IsNullOrEmpty(name)) name = "Ledger_" + DateTime.Now.ToString("yyyyMMddHHmmss");

        using (var connection = factory.CreateConnection()!)
        {
            await connection.OpenAsync();

            try
            {
                var id = Guid.NewGuid().ToString();
                var ledger = new LedgerEntity(id, name!, DateTime.Now);

                await connection.ExecuteAsync(
                    sql: "INSERT INTO Ledgers (ledger_id, name, last_modified) VALUES (@Id, @Name, @LastModified)",
                    new
                    {
                        Id = id, Name = name, LastModified = DateTime.Now
                    }
                );

                return Result.Ok(ledger);
            }
            catch (InvalidOperationException e)
            {
                logger.LogError(e, message: "Failed to create ledger");

                return Result.Fail<LedgerEntity>(e);
            }
            finally
            {
                connection.Close();
            }
        }
    }

    /// <inheritdoc />
    public async Task<Result<LedgerEntity>> GetLedgerByIdAsync(string id)
    {
        using (var connection = factory.CreateConnection()!)
        {
            await connection.OpenAsync();

            try
            {
                return await connection.QueryFirstAsync<LedgerEntity>(
                    sql: "SELECT (ledger_id, name, last_modified) FROM Ledgers WHERE ledger_id = @Id",
                    new
                    {
                        Id = id
                    }
                ).ToResultAsync();
            }
            catch (InvalidOperationException e)
            {
                logger.LogError(
                    e,
                    message: "Attempted to obtain ledger {LedgerId}, but no ledger by that id exists", id
                );

                return Result.Fail<LedgerEntity>(e);
            }
            finally
            {
                connection.Close();
            }
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<LedgerEntity>> GetLedgersAsync()
    {
        using (var connection = factory.CreateConnection()!)
        {
            await connection.OpenAsync();

            try
            {
                return (await connection.QueryAsync<LedgerEntity>(
                    "SELECT (ledger_id, name, last_modified) FROM Ledgers"
                )).ToImmutableList();
            }
            catch (InvalidOperationException e)
            {
                logger.LogError(e, message: "Failed to obtain list of ledgers");

                return ImmutableList<LedgerEntity>.Empty;
            }
            finally
            {
                connection.Close();
            }
        }
    }

    /// <inheritdoc />
    public async Task<Result> RenameAsync(string id, string name)
    {
        using (var connection = factory.CreateConnection()!)
        {
            await connection.OpenAsync();

            try
            {
                await connection.ExecuteAsync(
                    sql: "UPDATE Ledgers SET name = @Name, last_modified = @LastModified WHERE ledger_id = @Id",
                    new
                    {
                        Id = id, Name = name, LastModified = DateTime.Now
                    }
                );

                return Result.Ok();
            }
            catch (InvalidOperationException e)
            {
                logger.LogError(e, message: "Failed to rename ledger {LedgerId}", id);

                return Result.Fail<LedgerEntity>(e);
            }
            finally
            {
                connection.Close();
            }
        }
    }

    /// <inheritdoc />
    public async Task<Result> UpdateAsync(LedgerEntity entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Id)) throw new ArgumentException("Invalid ledger id");

        using (var connection = factory.CreateConnection()!)
        {
            await connection.OpenAsync();

            try
            {
                var rowsAffected = await connection.ExecuteAsync(
                    sql: """
                         UPDATE Ledgers 
                         SET name = @Name, 
                             last_modified = @LastModified 
                         WHERE ledger_id = @Id
                             AND last_modified = @LastModified
                         """,
                    new
                    {
                        entity.Id, entity.Name, entity.LastModified
                    }
                );

                return rowsAffected > 0 ? Result.Ok() : Result.Fail("No rows affected");
            }
            catch (InvalidOperationException e)
            {
                logger.LogError(e, message: "Failed to update ledger {LedgerId}", entity.Id);

                return Result.Fail<LedgerEntity>(e);
            }
            finally
            {
                connection.Close();
            }
        }
    }

    /// <inheritdoc />
    public async Task<Result> DeleteAsync(string id)
    {
        using (var connection = factory.CreateConnection()!)
        {
            await connection.OpenAsync();

            try
            {
                await connection.ExecuteAsync(
                    sql: "DELETE FROM Ledgers WHERE ledger_id = @Id",
                    new
                    {
                        Id = id
                    }
                );

                return Result.Ok();
            }
            catch (InvalidOperationException e)
            {
                logger.LogError(e, message: "Failed to delete ledger {LedgerId}", id);

                return Result.Fail<LedgerEntity>(e);
            }
            finally
            {
                connection.Close();
            }
        }
    }

    /// <inheritdoc />
    public async Task<Result<LedgerEntity>> GetLedgerByNameAsync(string name)
    {
        using (var connection = factory.CreateConnection()!)
        {
            await connection.OpenAsync();

            try
            {
                return await connection.QueryFirstAsync<LedgerEntity>(
                    sql: "SELECT (ledger_id, name, last_modified) FROM Ledgers WHERE name = @Name",
                    new
                    {
                        Name = name
                    }
                ).ToResultAsync();
            }
            catch (InvalidOperationException e)
            {
                logger.LogError(
                    e, message: "Attempted to obtain ledger {LedgerName}, but no ledger by that name exists", name
                );

                return Result.Fail<LedgerEntity>(e);
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
