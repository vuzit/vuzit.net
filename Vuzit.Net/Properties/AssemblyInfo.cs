using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
#if (NET_2_0)
[assembly: AssemblyTitle("Vuzit.Net for .NET Framework 2.0")]
#elif (NET_3_5)
[assembly: AssemblyTitle("Vuzit.Net for .NET Framework 3.5")]
#elif (MONO_2_2)
[assembly: AssemblyTitle("Vuzit.Net for Mono 2.2")]
#elif (MONO_2_4)
[assembly: AssemblyTitle("Vuzit.Net for Mono 2.4")]
#elif (MONO_2_6)
[assembly: AssemblyTitle("Vuzit.Net for Mono 2.6")]
#else
[assembly: AssemblyTitle("Vuzit.Net")]
#endif

[assembly: AssemblyDescription("Vuzit.Net Web Services Library")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Vuzit LLC - http://vuzit.com/")]
[assembly: AssemblyProduct("Vuzit.Net")]
[assembly: AssemblyCopyright("Copyright © Vuzit LLC 2009-2010")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

#if STRONG
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFileAttribute(@"Vuzit.Net.key")]
#endif

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("19085bf6-bb1d-4a6d-a0ef-c2fff342f6cf")]

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
[assembly: AssemblyVersion("2.1.0")]
[assembly: AssemblyFileVersion("2.1.0")]
