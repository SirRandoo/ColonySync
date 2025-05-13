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

using System.Threading;
using System.Threading.Tasks;
using ColonySync.Common.Interfaces;
using ColonySync.Mod.Infrastructure.FileWorkers;
using ColonySync.Mod.Infrastructure.Logging;
using ColonySync.Mod.Infrastructure.Serializers;
using ColonySync.Mod.Shared.FileWorkers;
using ColonySync.Mod.Shared.Serializers;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mono.Data.Sqlite;
using Verse;

namespace ColonySync.Mod.Infrastructure;

/// <summary>Provides orchestration functionalities for the mod.</summary>
/// <remarks>
///     This class is initialized statically on startup and is used to maintain the
///     synchronization context for the main thread of the game. It acts as a central point for
///     managing synchronizations within the game’s modding ecosystem.
/// </remarks>
[StaticConstructorOnStartup]
[UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
internal sealed class Orchestration
{
    /// <summary>Holds an instance of the DI (Dependency Injection) host.</summary>
    /// <remarks>
    ///     This static field is used to initialize and manage the dependency injection host
    ///     throughout the lifecycle of the orchestration process.
    /// </remarks>
    private static IHost _diHost = null!;

    /// <summary>Manages the synchronization context for coordinating tasks with the game's main thread.</summary>
    /// <remarks>
    ///     This variable is critical for ensuring that asynchronous operations interface correctly
    ///     with the main thread of the game, maintaining thread safety and application stability. It must
    ///     be initialized on the game's main thread, typically using the game's synchronization mechanism.
    /// </remarks>
    internal static readonly SynchronizationContext SynchronizationContext = SynchronizationContext.Current;

    /// <summary>
    ///     Provides orchestration logic for the application. Initiates and manages the dependency
    ///     injection (DI) host and performs necessary configurations.
    /// </summary>
    static Orchestration()
    {
        Task.Factory.StartNew(PerformOrchestration, TaskCreationOptions.LongRunning);
    }

    /// <summary>Initializes the Dependency Injection (DI) container and starts the orchestration process.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private static async Task PerformOrchestration()
    {
        _diHost = Host.CreateDefaultBuilder()
            .ConfigureLogging(LoggingServiceExtensions.ConfigureNLog)
            .ConfigureServices((context, collection) =>
                {
                    collection.AddSingleton<IFilePathProvider, FilePathProvider>();
                    collection.AddSingleton<IJsonDataSerializer, JsonDataSerializer>();
                    collection.AddSingleton<ITomlDataSerializer, TomlDataSerializer>();
                    collection.AddSingleton<IDataSerializerFactory, DataSerializerFactory>();
                    collection.AddSingleton<IPersistableJsonFileWorker, PersistableJsonFileWorker>();
                    collection.AddSingleton<IPersistableTomlFileWorker, PersistableTomlFileWorker>();

                    collection.AddHttpClient();
                    collection.AddSingleton(SqliteFactory.Instance);
                    collection.AddSingleton<PersistableJsonFileWorker>();
                }
            )
            .Build();

        await _diHost.RunAsync();
    }
}