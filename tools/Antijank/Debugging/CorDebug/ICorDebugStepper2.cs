using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("C5B6E9C3-E7D1-4A8E-873B-7F047F0706F7")]
  [SuppressUnmanagedCodeSecurity]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  
  public interface ICorDebugStepper2 {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetJMC([In] int fIsJMCStepper);

  }

}