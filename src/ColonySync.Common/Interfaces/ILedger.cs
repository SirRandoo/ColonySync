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

using System;

namespace ColonySync.Common.Interfaces;

/// <summary>
///     Defines the structure and operations for a ledger that manages user data, tracks modifications, and supports
///     transactions.
/// </summary>
public interface ILedger : IIdentifiable
{
    /// <summary>Gets or sets the timestamp of the last modification to the ledger.</summary>
    DateTime LastModified { get; set; }

    /// <summary>
    ///     Represents a registry of user data associated with the ledger, allowing for management of user-related
    ///     information.
    /// </summary>
    IRegistry<IUserData> Data { get; set; }

    /// <summary>Retrieves user-specific data associated with a given user.</summary>
    /// <param name="user">The user for which the data is being requested.</param>
    /// <returns>The <see cref="IUserData" /> associated with the specified user if found; otherwise, null.</returns>
    IUserData? GetUserData(IUser user);

    /// <summary>Processes a transaction for the specified user within the ledger.</summary>
    /// <param name="user">The user performing the transaction.</param>
    /// <param name="transaction">The transaction object representing the user's transaction details.</param>
    void PerformTransaction(IUser user, ITransaction transaction);
}