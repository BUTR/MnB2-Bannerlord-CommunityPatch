using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("A0EFB28B-6EE2-4D7B-B983-A75EF7BEEDB8")]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  [PublicAPI]
  public interface IMethodMalloc {

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
    IntPtr Alloc([In] uint cb);

  }

}