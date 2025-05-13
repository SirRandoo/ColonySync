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

#if DEBUG

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Bogus;
using Bogus.DataSets;
using ColonySync.Common.Enums;
using ColonySync.Common.Interfaces;
using ColonySync.Common.Registries;
using JetBrains.Annotations;

namespace ColonySync.Mod.Api;

/// <summary>A class for generating pseudo-random mod data such as users, transactions, and messages.</summary>
[PublicAPI]
public sealed class PseudoDataGenerator
{
    private static readonly Lazy<PseudoDataGenerator> InternalInstance = new(() => new PseudoDataGenerator());

    private static readonly string[] ProductTypes = ["ITEM", "EVENT", "PAWN", "TRAIT", "BACKSTORY"];

    /// <summary>
    ///     Represents a static, readonly registry of platform instances used for pseudo-data generation in ColonySync.
    /// </summary>
    public static readonly IReadOnlyRegistry<IPlatform> Platforms = FrozenRegistry<IPlatform>.CreateInstance(
        [
            new PseudoPlatform(id: "ColonySync:Platforms/Twitch", name: "Twitch (DEBUG)"),
            new PseudoPlatform(id: "ColonySync:Platforms/Trovo", name: "Trovo (DEBUG)"),
            new PseudoPlatform(id: "ColonySync:Platforms/Kick", name: "Kick (DEBUG)")
        ]
    );

    private static readonly Internet Internet = new();
    private static readonly Randomizer Randomizer = new();
    private static readonly Date Date = new();
    private static readonly Lorem Lorem = new();

    private PseudoDataGenerator()
    {
    }

    /// <summary>Gets the singleton instance of the PseudoDataGenerator class.</summary>
    public static PseudoDataGenerator Instance => InternalInstance.Value;

    /// <summary>Returns a randomly generated ledger seeded with random data.</summary>
    /// <param name="viewerCount">The total number of viewers to populate the ledger with.</param>
    /// <param name="transactionCount">The length of each viewer's transaction history.</param>
    /// <returns>A ledger populated with the specified number of viewers and their respective transaction histories.</returns>
    public ILedger GeneratePseudoLedger(int viewerCount, int transactionCount)
    {
        return new PseudoLedger(Guid.NewGuid().ToString())
        {
            Name = Lorem.Sentence(wordCount: 1, range: 1),
            Data = MutableRegistry<IUserData>.CreateInstance(
                GeneratePseudoUsers(viewerCount, transactionCount)
                    .Select(u => new PseudoUserData(u))
                    .ToList()
            )
        };
    }

    /// <summary>Returns a collection of randomly generated transactions.</summary>
    /// <param name="count">The total number of transactions to generate.</param>
    /// <return>A collection of generated pseudo transactions.</return>
    public ITransaction[] GeneratePseudoTransactions(int count)
    {
        return Enumerable.Range(start: 0, count)
            .AsParallel()
            .Select(ITransaction (_) => new PseudoTransaction(
                    Guid.NewGuid().ToString(), Lorem.Sentence(wordCount: 1, range: 1),
                    Randomizer.ArrayElement(ProductTypes), Date.Past()
                )
                {
                    Amount = Randomizer.Int(1), Morality = Randomizer.ArrayElement(Morality.ListMoralities()), Refunded = Randomizer.Bool(0.1f)
                }
            )
            .ToArray();
    }

    /// <summary>Generates an array of pseudo users with seeded random data.</summary>
    /// <param name="count">The number of users to generate.</param>
    /// <param name="transactionCount">The number of transactions each user should have in their history. Defaults to 50.</param>
    /// <returns>An array of generated pseudo users.</returns>
    public IUser[] GeneratePseudoUsers(int count, int transactionCount = 50)
    {
        return Enumerable.Range(start: 0, count)
            .Select(IUser (_) =>
                {
                    var randomPlatformIndex = Randomizer.Number(Platforms.AllRegistrants.Count - 1);
                    var randomPlatform = Platforms.AllRegistrants[randomPlatformIndex];

                    return new PseudoUser(Guid.NewGuid().ToString(), randomPlatform.Id)
                    {
                        Name = Internet.UserName(), Roles = GetRandomPrivileges(), LastSeen = Date.Past()
                    };
                }
            )
            .ToArray();
    }

    /// <summary>Generates an array of pseudo messages with seeded random content and authors.</summary>
    /// <param name="count">The number of messages to generate.</param>
    /// <returns>An array of generated pseudo messages.</returns>
    public IMessage[] GeneratePseudoMessages(int count)
    {
        return Enumerable.Range(start: 0, count)
            .Select(IMessage (_) => new PseudoMessage(Guid.NewGuid().ToString(), Lorem.Sentence(range: 10))
                {
                    Author = GeneratePseudoUsers(1)[0]
                }
            )
            .ToArray();
    }

    /// <summary>Assigns a set of random privileges to a user.</summary>
    /// <returns>
    ///     A bitwise combination of one or more <see cref="UserRoles" /> values representing the user's assigned
    ///     privileges.
    /// </returns>
    private UserRoles GetRandomPrivileges()
    {
        return Randomizer.EnumValues<UserRoles>()
            .Aggregate(UserRoles.None, (current, privilege) => current | privilege);
    }

    /// <summary>Represents a pseudo user in the system for testing and development purposes.</summary>
    private sealed class PseudoUser(string id, string platform) : IUser
    {
        /// <inheritdoc />
        public string Id { get; init; } = id;

        /// <inheritdoc />
        public string Platform { get; init; } = platform;

        /// <inheritdoc />
        public required string Name { get; init; }

        /// <inheritdoc />
        public DateTime LastSeen { get; set; } = DateTime.UtcNow;

        /// <inheritdoc />
        public UserRoles Roles { get; set; } = UserRoles.None;
    }

    /// <summary>Represents a pseudo-randomly generated message within a platform.</summary>
    private sealed class PseudoMessage(string id, string content) : IMessage
    {
        /// <inheritdoc />
        public string Id { get; init; } = id;

        /// <inheritdoc />
        public string Content { get; init; } = content;

        /// <inheritdoc />
        public required IUser Author { get; init; }

        /// <inheritdoc />
        public DateTime ReceivedAt { get; init; } = DateTime.UtcNow;
    }

    /// <summary>
    ///     Represents a pseudo transaction with properties such as Id, Name, ProductId, Morality, OccurredAt, Amount, and
    ///     Refunded.
    /// </summary>
    private sealed class PseudoTransaction(string id, string name, string productId, DateTime occurredAt) : ITransaction
    {
        /// <inheritdoc />
        public string Id { get; init; } = id;

        /// <inheritdoc />
        public string Name { get; init; } = name;

        /// <inheritdoc />
        public string ProductId { get; set; } = productId;

        /// <inheritdoc />
        public Morality Morality { get; set; }

        /// <inheritdoc />
        public DateTime OccurredAt { get; set; } = occurredAt;

        /// <inheritdoc />
        public long Amount { get; set; }

        /// <inheritdoc />
        public bool Refunded { get; set; }
    }

    /// <summary>Represents a pseudo ledger which implements ILedger interface, holding user data and modification timestamp.</summary>
    private sealed class PseudoLedger(string id) : ILedger
    {
        /// <inheritdoc />
        public string Id { get; init; } = id;

        /// <inheritdoc />
        public required string Name { get; init; }

        public IUserData? GetUserData(IUser user)
        {
            throw new NotImplementedException();
        }

        public void PerformTransaction(IUser user, ITransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public DateTime LastModified { get; set; }

        public IRegistry<IUserData> Data { get; set; } = SynchronisedRegistry<IUserData>.CreateInstance([]);
    }

    /// <summary>Represents a pseudo-platform implementation used for generating mock platform data such as IDs and names.</summary>
    private sealed class PseudoPlatform(string id, string name) : IPlatform
    {
        /// <inheritdoc />
        public string Id { get; init; } = id;

        /// <inheritdoc />
        public string Name { get; init; } = name;

        /// <inheritdoc />
        public byte[] IconData { get; set; } = [];

        /// <inheritdoc />
        public PlatformFeatures Features { get; } = PlatformFeatures.None;
    }

    public sealed class PseudoUserData(IUser user) : IUserData
    {
        public string Id
        {
            get => User.Id;
            init => throw new ReadOnlyException("Cannot set Id for PseudoUserData.");
        }

        public string Name
        {
            get => User.Name;
            init => throw new ReadOnlyException("Cannot set Name for PseudoUserData.");
        }
        public IUser User { get; set; } = user;
        public long Points { get; set; }
        public short Karma { get; set; }
        public List<ITransaction> Transactions { get; set; } = [];
    }
}

#endif
