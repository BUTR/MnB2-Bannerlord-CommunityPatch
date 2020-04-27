using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [ComImport]
  [ComConversionLoss]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("9B2C54E4-119F-4D6F-B402-527603266D69")]
  
  public interface ICorDebugProcess7 {

    [MethodImpl(MethodImplOptions.InternalCall)]
    // ReSharper disable once IdentifierTypo // yes it should writable
    void SetWriteableMetadataUpdateMode(
      [In] SetWriteableMetadataUpdateMode flags
    );

  }

}