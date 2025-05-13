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
using ColonySync.Common.Registries;
using JetBrains.Annotations;
using Verse;

namespace ColonySync.Mod.Api.Events;

/// <summary>A micro-class for housing event dispatching logic for specific events.</summary>
/// <typeparam name="TEvent">The type of the event that'll be dispatched.</typeparam>
public class PlatformEventDispatcher<TEvent> where TEvent : PlatformEvent
{
    private static readonly FrozenRegistry<IChannelEventHandler<TEvent>> Handlers;

    static PlatformEventDispatcher()
    {
        var handlers = new List<IChannelEventHandler<TEvent>>();

        foreach (var type in typeof(IChannelEventHandler<TEvent>).AllSubclassesNonAbstract())
        {
            IChannelEventHandler<TEvent>? instance;

            try
            {
                instance = Activator.CreateInstance(type) as IChannelEventHandler<TEvent>;
            }
            catch (Exception)
            {
                // Logger(e, "Could not instantiate event handler '{QualifiedName}'", type.FullDescription());

                continue;
            }

            handlers.Add(instance!);
        }

        Handlers = FrozenRegistry<IChannelEventHandler<TEvent>>.CreateInstance(handlers);
    }

    /// <summary>Dispatches a platform event to all registered event handlers.</summary>
    /// <param name="event">The event to dispatch.</param>
    [PublicAPI]
    public static async Task DispatchAsync(TEvent @event)
    {
        var handlers = Handlers.AllRegistrants;

        for (var i = 0; i < handlers.Count; i++)
        {
            var response = await handlers[i].HandleEvent(@event);

            if (response is ChannelEventResponse.Abort) break;
        }
    }
}
