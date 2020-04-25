using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [ComImport]
  [ComConversionLoss]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("21e9d9c0-fcb8-11df-8cff-0800200c9a66")]
  [PublicAPI]
  public interface ICorDebugProcess5 {

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("pHeapInfo")]
    COR_HEAPINFO GetGCHeapInformation();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppObjects")]
    ICorDebugHeapEnum EnumerateHeap();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppRegions")]
    ICorDebugHeapSegmentEnum EnumerateHeapRegions();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppObject")]
    ICorDebugObjectValue GetObject([In] ulong addr);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppEnum")]
    ICorDebugGCReferenceEnum EnumerateGCReferences(
      [MarshalAs(UnmanagedType.Bool)] [In] bool bEnumerateWeakReferences);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppEnum")]
    ICorDebugGCReferenceEnum EnumerateHandles([In] CorGCReferenceType types);

    // This is used for fast Heap dumping.
    // You have to keep track of field layout but you can bulk copy everything.

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("pId")]
    COR_TYPEID GetTypeID([In] ulong objAddr);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("type")]
    ICorDebugType GetTypeForTypeID([In] COR_TYPEID id);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("layout")]
    COR_ARRAY_LAYOUT GetArrayLayout([In] COR_TYPEID id);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("layout")]
    COR_TYPE_LAYOUT GetTypeLayout([In] COR_TYPEID id);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetTypeFields(
      [In] COR_TYPEID id,
      int celt,
      [Out] [MarshalAs(UnmanagedType.LPArray)]
      COR_FIELD[] fields,
      [Out] out int pceltNeeded);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnableNGENPolicy(CorDebugNGENPolicyFlags ePolicy);

  }

}