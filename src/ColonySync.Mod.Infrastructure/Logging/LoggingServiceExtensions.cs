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
using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using NLog.Targets.Wrappers;
using Verse;
using LogLevel = NLog.LogLevel;

namespace ColonySync.Mod.Infrastructure.Logging;

/// <summary>Provides extension methods for configuring logging services.</summary>
internal static class LoggingServiceExtensions
{
    /// <summary>Defines the layout format for log messages to be used in NLog targets.</summary>
    /// <remarks>
    ///     This layout string specifies the structure of log messages, including the timestamp, log
    ///     level, thread ID, logger name, and the actual log message. The exception details are also
    ///     included if present. Format: ${time} [${level:uppercase=true}][Thread ${threadid}] (${logger})
    ///     :: ${message:withexception=true}
    /// </remarks>
    private const string LogMessageLayout =
        "${time} [${level:uppercase=true}][Thread ${threadid}] (${logger}) :: ${message:withexception=true}";

    /// <summary>Configures NLog as the logging provider for the application.</summary>
    /// <param name="context">The HostBuilderContext for accessing application settings.</param>
    /// <param name="builder">The ILoggingBuilder for configuring logging services.</param>
    internal static void ConfigureNLog(HostBuilderContext context, ILoggingBuilder builder)
    {
        var configuration = BuildNLogConfiguration();
        InjectConsoleTarget(configuration);

        builder.AddNLog(configuration);
    }

    /// <summary>Builds the NLog configuration for the logging system.</summary>
    /// <returns>A configured <c>NLog.Config.LoggingConfiguration</c> object.</returns>
    private static LoggingConfiguration BuildNLogConfiguration()
    {
        var config = new LoggingConfiguration();
        var playerLogTarget = new PlayerLogTarget
        {
            Layout = LogMessageLayout
        };
        var asyncTargetWrapper = new AsyncTargetWrapper(playerLogTarget);

        config.AddTarget(asyncTargetWrapper);
        config.AddRule(LogLevel.Warn, LogLevel.Fatal, playerLogTarget);

        return config;
    }

    /// <summary>
    ///     Injects a console target into the provided logging configuration, but only if running in
    ///     debug mode and specific conditions are met.
    /// </summary>
    /// <param name="config">The logging configuration to inject the console target into.</param>
    [Conditional("DEBUG")]
    private static void InjectConsoleTarget(LoggingConfiguration config)
    {
        if (!GenCommandLine.TryGetCommandLineArg(key: "-logfile", out var value) &&
            !string.Equals(value, b: "-", StringComparison.Ordinal)) return;

        var consoleTarget = new ColoredConsoleTarget
        {
            Layout = LogMessageLayout
        };
        var consoleTargetWrapper = new BufferingTargetWrapper(consoleTarget);

        config.AddTarget(consoleTargetWrapper);
        config.AddRule(LogLevel.Trace, LogLevel.Fatal, consoleTargetWrapper);
    }
}