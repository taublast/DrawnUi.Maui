global using System;
global using System.Threading;
global using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

// IsExternalInit.cs
#if NETSTANDARD || NETCOREAPP3_1 || NETFRAMEWORK
namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Allows the use of the 'init' accessor and records in projects targeting older frameworks.
    /// </summary>
    public static class IsExternalInit { }
}
#endif
