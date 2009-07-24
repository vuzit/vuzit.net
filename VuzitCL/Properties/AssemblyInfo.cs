using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
#if (NET_2_0)
[assembly: AssemblyTitle("VuzitCL command line for .NET Framework 2.0")]
#elif (NET_3_5)
[assembly: AssemblyTitle("VuzitCL command line for .NET Framework 3.5")]
#elif (MONO_2_2)
[assembly: AssemblyTitle("VuzitCL command line for Mono 2.2")]
#else
[assembly: AssemblyTitle("VuzitCL - Vuzit Web services command line")]
#endif

[assembly: AssemblyDescription("Command line application for the Vuzit Web Services layer")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Vuzit LLC - http://vuzit.com/")]
[assembly: AssemblyProduct("VuzitCL")]
[assembly: AssemblyCopyright("Copyright © Vuzit LLC 2009")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("da6718e8-bba1-4c14-bd7f-f5c5813c19fa")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0")]
[assembly: AssemblyFileVersion("1.0.0")]
