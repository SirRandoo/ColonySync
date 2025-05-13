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
using System.Xml.Serialization;
using NetEscapades.EnumGenerators;

namespace ColonySync.Mod.Bootloader;

/// <summary>Represents a collection of resource bundles in the mod's bootloader system.</summary>
public class Corpus(ResourceBundle[] resources)
{
    /// <summary>
    ///     Contains an array of ResourceBundle objects, each representing a bundle of resources that
    ///     can be loaded and processed by the application.
    /// </summary>
    [XmlArrayItem(typeof(ResourceBundle))]
    public ResourceBundle[] Resources { get; } = resources ?? throw new ArgumentNullException(nameof(resources));
}

/// <summary>
///     Represents a collection of resources within a specified root directory, optionally
///     versioned, and encompassing multiple individual resources.
/// </summary>
public class ResourceBundle(string root, bool versioned, Resource[] resources)
{
    /// <summary>
    ///     Specifies the root directory for the resource bundle. This property holds the base path
    ///     from where the resources of the bundle will be located or processed.
    /// </summary>
    [XmlAttribute]
    public string Root { get; } = root ?? throw new ArgumentNullException(nameof(root));

    /// <summary>
    ///     Indicates whether the resource bundle supports versioning, influencing how the resources
    ///     are managed and loaded.
    /// </summary>
    [XmlAttribute]
    public bool Versioned { get; } = versioned;

    /// <summary>
    ///     Contains the list of resources defined in the resource bundle, which can be processed and
    ///     loaded by the application.
    /// </summary>
    [XmlElement(elementName: "Resource", typeof(Resource))]
    public Resource[] Resources { get; } = resources ?? throw new ArgumentNullException(nameof(resources));
}

/// <summary>Represents a file on disk.</summary>
public class Resource(ResourceType type, string name, bool optional, string root)
{
    /// <summary>
    ///     Specifies the type of the resource, defining how it should be processed and loaded by the
    ///     application.
    /// </summary>
    [XmlAttribute]
    public ResourceType Type { get; init; } = type;

    /// <summary>The name of the resource file.</summary>
    [XmlAttribute]
    public string Name { get; init; } = name ?? throw new ArgumentNullException(nameof(name));

    /// <summary>Indicates whether the resource is optional.</summary>
    [XmlAttribute]
    public bool Optional { get; init; } = optional;

    /// <summary>
    ///     Specifies the root directory for the resource. This path is typically combined with the
    ///     overall path to correctly locate the resource on disk.
    /// </summary>
    [XmlAttribute]
    public string Root { get; init; } = root ?? throw new ArgumentNullException(nameof(root));
}

/// <summary>Specifies different types of resources that the mod's bootloader can load.</summary>
[EnumExtensions]
public enum ResourceType
{
    /// <summary>Represents a dynamic-link library (DLL) resource type.</summary>
    [XmlEnum(Name = "Dll")] Dll,

    /// <summary>Represents an assembly that can be loaded by the mod's bootloader.</summary>
    [XmlEnum(Name = "Assembly")] Assembly,

    /// <summary>Represents a .NET Standard assembly, which is compatible with multiple .NET platforms.</summary>
    [XmlEnum(Name = "NetStandardAssembly")]
    NetStandardAssembly
}