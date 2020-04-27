using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("CC7BCAEC-8A68-11D2-983C-0000F808342D")]
  [ComImport]
  
  public interface ICorDebugStepper {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void IsActive(out int pbActive);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Deactivate();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetInterceptMask([In] CorDebugIntercept mask);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetUnmappedStopMask([In] CorDebugUnmappedStop mask);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Step([In] int bStepIn);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StepRange([In] int bStepIn, [In] ref COR_DEBUG_STEP_RANGE ranges, [In] uint cRangeCount);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StepOut();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetRangeIL([In] int bIL);

  }

}