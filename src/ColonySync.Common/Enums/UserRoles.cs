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
using NetEscapades.EnumGenerators;

namespace ColonySync.Common.Enums;

/// <summary>Indicates the roles assigned to users within the chat system of a platform.</summary>
[Flags]
[EnumExtensions]
public enum UserRoles
{
    /// <summary>Denotes that the user does not have any assigned roles within the chat system.</summary>
    None = 0,

    /// <summary>Used to indicate that the user holds a VIP status within the chat.</summary>
    Vip = 1,

    /// <summary>Used to indicate that the user has a subscription within the chat system.</summary>
    Subscriber = 2,

    /// <summary>
    ///     Used to indicate that the user is responsible for overseeing the chat, including
    ///     moderating discussions and managing user interactions.
    /// </summary>
    Moderator = 4
}