using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;


namespace Antijank.Debugging {

  [Guid("AA8FA804-BC05-4642-B2C5-C353ED22FC63")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public interface ICLRMetadataLocator {

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void GetMetadata([MarshalAs(UnmanagedType.LPWStr)] [In] string imagePath, [In] uint imageTimestamp,
      [In] uint imageSize, [In] [MarshalAs(UnmanagedType.LPStruct)]
      Guid mvId, [In] uint mdRva, [In] uint flags, [In] uint bufferSize, ref byte* buffer, out uint dataSize);

  }

}