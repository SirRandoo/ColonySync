using ColonySync.Common.Interfaces;
using JetBrains.Annotations;

namespace ColonySync.Mod.Api.Events;

/// <summary>
///     Represents a raid from another channel.
/// </summary>
/// <inheritdoc cref="PlatformEvent" path="/param[@name='Platform' or 'User']" />
/// <param name="Raiders">
///     The number of people that raided the channel with the streamer who raided the
///     channel.
/// </param>
[PublicAPI]
public sealed record ChannelRaidedEvent(IPlatform Platform, IUser User, int Raiders) : PlatformEvent(Platform, User);