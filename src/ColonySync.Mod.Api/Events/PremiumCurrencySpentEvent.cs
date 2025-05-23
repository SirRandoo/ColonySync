using ColonySync.Common.Interfaces;
using JetBrains.Annotations;

namespace ColonySync.Mod.Api.Events;

/// <summary>
///     Represents a user spending a platform's premium currency in the channel.
/// </summary>
/// <inheritdoc cref="PlatformEvent" path="/param[@name='Platform' or 'User']" />
/// <param name="Amount">The amount of premium currency the user spent in the channel.</param>
/// <param name="Message">The optional message the user sent.</param>
[PublicAPI]
public sealed record PremiumCurrencySpentEvent(IPlatform Platform, IUser User, int Amount, string? Message = null)
    : PlatformEvent(Platform, User);