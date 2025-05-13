using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ColonySync.Mod.Api;

/// <summary>
///     Provides methods for indexing and retrieving instances of a specified type from the current application
///     domain.
/// </summary>
internal sealed class DomainIndexer(ILogger logger)
{
    private const string CreateInstanceMethodName = "CreateInstance";
    private const BindingFlags CreateInstanceBindingFlags = BindingFlags.Static | BindingFlags.Public;

    private static readonly Type[] EmptyTypes = [];
    private static readonly HashSet<Assembly> ProblematicAssemblies = [];

    /// <summary>
    ///     Indexes and retrieves a list of instances of a specified type <typeparamref name="T" /> from the current
    ///     application domain.
    /// </summary>
    /// <typeparam name="T">The type to index instances of. Must be a class.</typeparam>
    /// <returns>
    ///     A list of instances of the specified type found in the current application domain. If an error occurs during
    ///     indexing, an empty list is returned.
    /// </returns>
    internal IList<T> IndexFor<T>() where T : class
    {
        List<T> results;

        try
        {
            results = AppDomain.CurrentDomain.GetAssemblies()
                .AsParallel()
                .SelectMany((assembly, _) => GetAssemblyTypes(assembly))
                .Where(IsValidType<T>)
                .Select(CreateInstance<T>)
                .Where(c => c != null)
                .ToList()!;
        }
        catch (Exception e)
        {
            logger.LogError(e, message: "Encountered an exception while indexing the current app domain");
            results = [];
        }

        logger.LogWarning(
            message: "Found {Total} instances of {Type} in the current domain", results.Count,
            typeof(T).FullDescription()
        );

        return results;
    }

    /// <summary>Retrieves all types defined in a specified assembly.</summary>
    /// <param name="assembly">The assembly from which to retrieve types.</param>
    /// <returns>An array of types defined in the specified assembly. If an error occurs, an empty array is returned.</returns>
    private Type[] GetAssemblyTypes(Assembly assembly)
    {
        if (ProblematicAssemblies.Contains(assembly))
        {
            logger.LogDebug(
                message:
                "Skipping assembly {Assembly} ({AssemblyPath}) as it previously threw an exception while getting its types",
                assembly.FullName, assembly.Location
            );

            return EmptyTypes;
        }

        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException e)
        {
            logger.LogError(e, message: "The assembly previously threw an exception while being loaded by the runtime");

            for (var i = 0; i < e.LoaderExceptions.Length; i++)
                logger.LogError(
                    e.LoaderExceptions[i],
                    message: "The method '{Method}' threw an exception during the loading process",
                    e.LoaderExceptions[i].TargetSite
                );

            return EmptyTypes;
        }
        catch (Exception e)
        {
            logger.LogError(
                e, message: "Could not get types from the assembly {Assembly} ({AssemblyPath})", assembly.FullName,
                assembly.Location
            );

            ProblematicAssemblies.Add(assembly);

            return EmptyTypes;
        }
    }

    /// <summary>Determines if a given type is a valid type for the specified type parameter <typeparamref name="T" />.</summary>
    /// <typeparam name="T">The type to validate against. Must be a class.</typeparam>
    /// <param name="type">The type to validate.</param>
    /// <returns>Returns true if the given type is valid; otherwise, false.</returns>
    private static bool IsValidType<T>(Type type)
    {
        if (type.IsInterface) return false;

        if (type.IsAbstract) return false;

        if (type.Namespace == null
            || type.Namespace.StartsWith(value: "System", StringComparison.Ordinal)
            || type.Namespace.StartsWith(value: "Unity", StringComparison.Ordinal)
            || type.Namespace.StartsWith(value: "Verse", StringComparison.Ordinal)
            || type.Namespace.StartsWith(value: "RimWorld", StringComparison.Ordinal))
            return false;

        if (!typeof(T).IsAssignableFrom(type)) return false;

        if (type.GetMethod(CreateInstanceMethodName, CreateInstanceBindingFlags) != null) return true;

        return Array.TrueForAll(
            type.GetConstructors(),
            constructor => !Array.Exists(
                constructor.GetParameters(), parameter => !parameter.HasDefaultValue
            )
        );
    }

    /// <summary>
    ///     Creates an instance of the specified type <typeparamref name="T" />. If the type has a static method named
    ///     "CreateInstance", it will invoke that method to create the instance. Otherwise, it will attempt to use the default
    ///     constructor to create the instance.
    /// </summary>
    /// <typeparam name="T">The type for which an instance should be created. Must be a class.</typeparam>
    /// <param name="type">The Type object representing the class type to instantiate.</param>
    /// <returns>An instance of the specified type, or null if creation fails.</returns>
    private T? CreateInstance<T>(Type type) where T : class
    {
        var method = type.GetMethod(CreateInstanceMethodName, CreateInstanceBindingFlags);

        if (method == null)
            try
            {
                return Activator.CreateInstance(type) as T;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, message: "Could not create an instance of {Type}", type.FullDescription());

                return null;
            }

        try
        {
            return method.Invoke(obj: null, []) as T;
        }
        catch (Exception e)
        {
            logger.LogWarning(
                e, message: "Could not constructor type '{Type}' from method '{MethodName}'", type.FullDescription(),
                CreateInstanceMethodName
            );

            return null;
        }
    }
}