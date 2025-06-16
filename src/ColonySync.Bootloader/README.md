# ColonySync.Bootloader

The bootloader is an orchestration project for ColonySync. Its
sole purpose is to load assemblies for ColonySync in a manner
supported by Unity Engine. The details of this orchestration are
as follows:

- .NET Framework and .NET Standard assemblies are loaded by `AppDomain.CurrentDomain.Load(byte[], byte[])`
- Native DLLs are loaded by Unity’s variant of Mono through environment variable path extensions.
