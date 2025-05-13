using System.Collections.Generic;

namespace ColonySync.Common.Interfaces;

/// <summary>Provides services for managing users within ledgers.</summary>
public interface ILedgerService
{
    /// <summary>Adds a user to a ledger.</summary>
    /// <param name="user">The user to be added to the ledger.</param>
    /// <param name="ledger">The ledger to which the user will be added.</param>
    void AddUserToLedger(IUser user, ILedger ledger);

    /// <summary>Removes a user from a ledger.</summary>
    /// <param name="user">The user to be removed from the ledger.</param>
    /// <param name="ledger">The ledger from which the user will be removed.</param>
    void RemoveUserFromLedger(IUser user, ILedger ledger);

    /// <summary>Returns the ledgers a user belongs to.</summary>
    /// <param name="user">The user whose ledgers are to be retrieved.</param>
    /// <returns>A list of ledgers that the user is part of.</returns>
    List<ILedger> GetLedgersOfUser(IUser user);

    /// <summary>Returns the users of a ledger.</summary>
    /// <param name="ledger">The ledger from which to retrieve the users.</param>
    /// <returns>A list of users associated with the specified ledger.</returns>
    List<IUser> GetUsersOfLedger(ILedger ledger);
}