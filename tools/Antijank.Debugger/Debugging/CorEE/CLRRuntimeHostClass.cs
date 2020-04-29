using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;


namespace Antijank.Debugging {

  [ClassInterface(ClassInterfaceType.None)]
  [SuppressUnmanagedCodeSecurity]
  [Guid("90F1A06E-7712-4762-86B5-7A5EBA6BDB02")]
  [ComImport]
  
  public class CLRRuntimeHostClass : CLRRuntimeHost, ICLRValidator {

    //[MethodImpl(MethodImplOptions.InternalCall)]
    //public extern CLRRuntimeHostClass();
    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void Start();

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void Stop();

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void SetHostControl([MarshalAs(UnmanagedType.Interface)] [In]
      IHostControl pHostControl);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void GetCLRControl([MarshalAs(UnmanagedType.Interface)] out ICLRControl pCLRControl);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void UnloadAppDomain([In] uint dwAppDomainID, [In] int fWaitUntilDone);

    /* TODO: callback
    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void ExecuteInAppDomainCallback(IntPtr cookie);
    */
    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void ExecuteInAppDomain([In] uint dwAppDomainID, [MarshalAs(UnmanagedType.Interface)] [In]
      CLRRuntimeHost pCallback, [In] IntPtr cookie);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void GetCurrentAppDomainId(out uint pdwAppDomainId);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void ExecuteApplication([MarshalAs(UnmanagedType.LPWStr)] [In] string pwzAppFullName,
      [In] uint dwManifestPaths, [MarshalAs(UnmanagedType.LPWStr)] [In] ref string ppwzManifestPaths,
      [In] uint dwActivationData, [MarshalAs(UnmanagedType.LPWStr)] [In] ref string ppwzActivationData,
      out int pReturnValue);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void ExecuteInDefaultAppDomain([MarshalAs(UnmanagedType.LPWStr)] [In] string pwzAssemblyPath,
      [MarshalAs(UnmanagedType.LPWStr)] [In] string pwzTypeName,
      [MarshalAs(UnmanagedType.LPWStr)] [In] string pwzMethodName,
      [MarshalAs(UnmanagedType.LPWStr)] [In] string pwzArgument, out uint pReturnValue);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void Validate([MarshalAs(UnmanagedType.Interface)] [In]
      IVEHandler veh, [In] uint ulAppDomainId, [In] uint ulFlags, [In] uint ulMaxError, [In] uint Token,
      [MarshalAs(UnmanagedType.LPWStr)] [In] string fileName, [In] ref byte pe, [In] uint ulSize);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void FormatEventInfo([MarshalAs(UnmanagedType.Error)] [In] int hVECode, [In] VerError Context,
      [MarshalAs(UnmanagedType.LPWStr)] [In] [Out]
      StringBuilder msg, [In] uint ulMaxLength,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)] [In]
      object[] psa);

  }

}