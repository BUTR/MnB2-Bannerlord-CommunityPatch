using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("86F012BF-FF15-4372-BD30-B6F11CAAE1DD")]
  [SuppressUnmanagedCodeSecurity]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  
  public interface ICorDebugModule3 {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void CreateReaderForInMemorySymbols([In] [MarshalAs(UnmanagedType.LPStruct)]
      Guid riid, out IntPtr ppObj);

  }

}