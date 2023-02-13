using System.Reflection;
using System.Runtime.InteropServices;

#if NETFRAMEWORK
[assembly: AssemblyTitle(".NET Framework")]
#endif

#if NETCOREAPP
[assembly: AssemblyTitle(".NET Core")]
#endif

#if NETSTANDARD
[assembly: AssemblyTitle(".NET Standard")]
#endif

[assembly: AssemblyVersion("1.4.0.0")]
[assembly: AssemblyCopyright("Copyright (c) 2023, Eben Roux")]
[assembly: AssemblyProduct("Shuttle.Core.Mediator.OpenTelemetry")]
[assembly: AssemblyCompany("Eben Roux")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyInformationalVersion("1.4.0-rc.4")]
[assembly: ComVisible(false)]