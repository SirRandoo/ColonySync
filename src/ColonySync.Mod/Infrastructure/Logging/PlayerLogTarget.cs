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

using NLog;
using NLog.Targets;
using Verse;

namespace ColonySync.Mod.Infrastructure.Logging;

/// <summary>Represents a custom NLog target that sends log messages to the game's log system.</summary>
/// <remarks>
///     This class directs log messages to different log methods based on their severity.
///     <list type="bullet">
///         <item>Warning, Fatal, and Error messages are posted to the game's warning log method.</item>
///         <item>Other messages are posted to the game's regular log method.</item>
///     </list>
/// </remarks>
[Target(TargetName)]
internal class PlayerLogTarget : TargetWithContext
{
    private const string TargetName = "PlayerLog";

    protected override void Write(LogEventInfo logEvent)
    {
        var logMessage = RenderLogEvent(Layout, logEvent);

        if (logEvent.Level == LogLevel.Warn || logEvent.Level == LogLevel.Fatal || logEvent.Level == LogLevel.Error)
            Orchestration.SynchronizationContext.Post(LogRedMessage, logMessage);
        else
            Orchestration.SynchronizationContext.Post(LogMessage, logMessage);
    }

    private static void LogMessage(object logMessage)
    {
        Log.Message((string)logMessage);
    }

    private static void LogRedMessage(object logMessage)
    {
        Log.Warning((string)logMessage);
    }
}