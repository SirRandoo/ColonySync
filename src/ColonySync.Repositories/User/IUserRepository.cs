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
using System.Threading.Tasks;
using ColonySync.Common;
using ColonySync.Common.Enums;

namespace ColonySync.Repositories.User;

/// <summary>
///     Represents the operations available for managing users in the repository.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    ///     Asynchronously creates a new user and saves it to the repository.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="platform">The platform on which the user resides.</param>
    /// <param name="name">The name of the user.</param>
    /// <param name="coins">The initial number of coins the user has. Defaults to 0.</param>
    /// <param name="karma">The initial karma score of the user. Defaults to 0.</param>
    /// <param name="lastSeen">The timestamp of the user's last activity. Defaults to the default DateTime value.</param>
    /// <returns>
    ///     A result containing the created <see cref="UserEntity" /> if the operation succeeded; otherwise, an error
    ///     result.
    /// </returns>
    Task<Result<UserEntity>> CreateUserAsync(string id, string platform, string name, long coins = 0, short karma = 0,
        DateTime lastSeen = default);

    /// <summary>
    ///     Retrieves a user by their unique identifier and platform.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="platform">The platform associated with the user.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a <see cref="Result{UserEntity}" />
    ///     which indicates the success or failure of the operation and, if successful, includes the retrieved user entity.
    /// </returns>
    Task<Result<UserEntity>> GetUserByIdAsync(string id, string platform);

    /// <summary>
    ///     Retrieves a user entity by their name and platform.
    /// </summary>
    /// <param name="name">The name of the user to retrieve.</param>
    /// <param name="platform">The platform associated with the user.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a
    ///     <see cref="Result{UserEntity}" /> indicating the operation's success and the associated user entity if found.
    /// </returns>
    Task<Result<UserEntity>> GetUserByNameAsync(string name, string platform);

    /// <summary>
    ///     Updates an existing user record in the database.
    /// </summary>
    /// <param name="entity">The updated user entity containing all relevant information.</param>
    /// <returns>
    ///     A <see cref="Result{UserEntity}" /> indicating the success or failure of the operation, along with the updated
    ///     user entity if the operation is successful.
    /// </returns>
    Task<Result<UserEntity>> UpdateUserAsync(UserEntity entity);

    /// Retrieves a read-only list of all user entities.
    /// <returns>
    ///     A task representing the asynchronous operation. The task result contains
    ///     a read-only list of UserEntity objects.
    /// </returns>
    Task<IReadOnlyList<UserEntity>> GetUsersAsync();

    /// <summary>
    ///     Renames a user by updating their name based on the specified ID and platform.
    /// </summary>
    /// <param name="id">The unique identifier of the user to be renamed.</param>
    /// <param name="platform">The platform to which the user belongs.</param>
    /// <param name="name">The new name to be assigned to the user.</param>
    /// <returns>
    ///     A <see cref="Result" /> indicating the success or failure of the operation.
    ///     Returns a successful result if the user's name was successfully updated,
    ///     otherwise returns a failure result with error details.
    /// </returns>
    Task<Result> RenameUserAsync(string id, string platform, string name);

    /// <summary>
    ///     Deletes a user by their unique identifier and platform identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    /// <param name="platform">The platform identifier associated with the user.</param>
    /// <returns>A <see cref="Result" /> indicating the success or failure of the operation.</returns>
    Task<Result> DeleteUserAsync(string id, string platform);
}

/// <summary>
///     Represents a user entity within the system, encapsulating user details and metadata.
/// </summary>
public sealed record UserEntity(
    string Id,
    string Platform,
    string Name,
    UserRoles Roles,
    long Coins,
    short Karma,
    DateTime LastSeen
);