using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("5C552AB6-FC09-4CB3-8E36-22FA03C798B7")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  [PublicAPI]
  public interface IXCLRDataProcess {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Flush();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartEnumTasks(out ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumTask([In] [Out] ref ulong handle, [MarshalAs(UnmanagedType.Interface)] out IXCLRDataTask task);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndEnumTasks([In] ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetTaskByOSThreadID([In] uint osThreadID, [MarshalAs(UnmanagedType.Interface)] out IXCLRDataTask task);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetTaskByUniqueID([In] ulong taskID, [MarshalAs(UnmanagedType.Interface)] out IXCLRDataTask task);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFlags(out uint flags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void IsSameObject([MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataProcess process);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetManagedObject([MarshalAs(UnmanagedType.Interface)] out IXCLRDataValue value);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetDesiredExecutionState(out uint state);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetDesiredExecutionState([In] uint state);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetAddressType([In] ulong address, out CLRDataAddressType type);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetRuntimeNameByAddress([In] ulong address, [In] uint flags, [In] uint bufLen, out uint nameLen,
      out ushort nameBuf, out ulong displacement);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartEnumAppDomains(out ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumAppDomain([In] [Out] ref ulong handle,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataAppDomain appDomain);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndEnumAppDomains([In] ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetAppDomainByUniqueID([In] ulong id, [MarshalAs(UnmanagedType.Interface)] out IXCLRDataAppDomain appDomain);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartEnumAssemblies(out ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumAssembly([In] [Out] ref ulong handle, [MarshalAs(UnmanagedType.Interface)] out IXCLRDataAssembly assembly);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndEnumAssemblies([In] ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartEnumModules(out ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumModule([In] [Out] ref ulong handle, [MarshalAs(UnmanagedType.Interface)] out IXCLRDataModule mod);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndEnumModules([In] ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetModuleByAddress([In] ulong address, [MarshalAs(UnmanagedType.Interface)] out IXCLRDataModule mod);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartEnumMethodInstancesByAddress([In] ulong address, [MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataAppDomain appDomain, out ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumMethodInstanceByAddress([In] ref ulong handle,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataMethodInstance method);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndEnumMethodInstancesByAddress([In] ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetDataByAddress([In] ulong address, [In] uint flags, [MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataAppDomain appDomain, [MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataTask tlsTask, [In] uint bufLen, out uint nameLen, out ushort nameBuf,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataValue value, out ulong displacement);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetExceptionStateByExceptionRecord([In] ref EXCEPTION_RECORD64 record,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataExceptionState exState);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void TranslateExceptionRecordToNotification([In] ref EXCEPTION_RECORD64 record,
      [MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataExceptionNotification notify);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void Request([In] uint reqCode, [In] uint inBufferSize, [In] ref byte* inBuffer, [In] uint outBufferSize,
      ref byte* outBuffer);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void CreateMemoryValue([MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataAppDomain appDomain, [MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataTask tlsTask, [MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataTypeInstance type, [In] ulong addr, [MarshalAs(UnmanagedType.Interface)] out IXCLRDataValue value);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetAllTypeNotifications([MarshalAs(UnmanagedType.Interface)] IXCLRDataModule mod, uint flags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetAllCodeNotifications([MarshalAs(UnmanagedType.Interface)] IXCLRDataModule mod, uint flags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetTypeNotifications([In] uint numTokens, [MarshalAs(UnmanagedType.Interface)] [In]
      ref IXCLRDataModule mods, [MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataModule singleMod, [In] ref uint tokens, out uint flags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetTypeNotifications([In] uint numTokens, [MarshalAs(UnmanagedType.Interface)] [In]
      ref IXCLRDataModule mods, [MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataModule singleMod, [In] ref uint tokens, [In] ref uint flags, [In] uint singleFlags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetCodeNotifications([In] uint numTokens, [MarshalAs(UnmanagedType.Interface)] [In]
      ref IXCLRDataModule mods, [MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataModule singleMod, [In] ref uint tokens, out uint flags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetCodeNotifications([In] uint numTokens, [MarshalAs(UnmanagedType.Interface)] [In]
      ref IXCLRDataModule mods, [MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataModule singleMod, [In] ref uint tokens, [In] ref uint flags, [In] uint singleFlags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetOtherNotificationFlags(out uint flags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetOtherNotificationFlags([In] uint flags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartEnumMethodDefinitionsByAddress([In] ulong address, out ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumMethodDefinitionByAddress([In] ref ulong handle,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataMethodDefinition method);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndEnumMethodDefinitionsByAddress([In] ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void FollowStub([In] uint inFlags, [In] ulong inAddr, [In] ref CLRDATA_FOLLOW_STUB_BUFFER inBuffer,
      out ulong outAddr, out CLRDATA_FOLLOW_STUB_BUFFER outBuffer, out uint outFlags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void FollowStub2([MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataTask task, [In] uint inFlags, [In] ulong inAddr, [In] ref CLRDATA_FOLLOW_STUB_BUFFER inBuffer,
      out ulong outAddr, out CLRDATA_FOLLOW_STUB_BUFFER outBuffer, out uint outFlags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void DumpNativeImage([In] ulong loadedBase, [MarshalAs(UnmanagedType.LPWStr)] [In] string name,
      [MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataDisplay display, [MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRLibrarySupport libSupport, [MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDisassemblySupport dis);

  }

}