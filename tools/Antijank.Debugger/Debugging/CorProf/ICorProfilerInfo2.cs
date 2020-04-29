using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;


namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [ComConversionLoss]
  [Guid("CC0935CD-A518-487D-B0BB-A93214E65478")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  
  public interface ICorProfilerInfo2 : ICorProfilerInfo {

    void DoStackSnapshot(UIntPtr thread, [In]
      IntPtr callback, [In] uint infoFlags, [In] IntPtr clientData, [In] ref byte context,
      [In] uint contextSize);

    void SetEnterLeaveFunctionHooks2(
      [In] IntPtr pFuncEnter,
      [In] IntPtr pFuncLeave,
      [In] IntPtr pFuncTailcall);

    void GetFunctionInfo2(UIntPtr funcID, UIntPtr frameInfo, out UIntPtr pClassId, out UIntPtr pModuleId,
      out int pToken, [In] uint cTypeArgs, out uint pcTypeArgs, out UIntPtr typeArgs);

    void GetStringLayout(out uint pBufferLengthOffset, out uint pStringLengthOffset, out uint pBufferOffset);

    void GetClassLayout(UIntPtr classId, [In] [Out] ref COR_FIELD_OFFSET rFieldOffset, [In] uint cFieldOffset,
      out uint pcFieldOffset, out uint pulClassSize);

    void GetClassIDInfo2(UIntPtr classId, out UIntPtr pModuleId, out int pTypeDefToken, out UIntPtr pParentClassId,
      [In] uint cNumTypeArgs, out uint pcNumTypeArgs, out UIntPtr typeArgs);

    void GetCodeInfo2(UIntPtr functionId, [In] uint cCodeInfos, out uint pcCodeInfos,
      [MarshalAs(UnmanagedType.Interface)] [Out]
      ICorProfilerInfo2 codeInfos);

    void GetClassFromTokenAndTypeArgs(UIntPtr moduleId, [In] int typeDef, [In] uint cTypeArgs, ref UIntPtr typeArgs,
      out UIntPtr pClassId);

    void GetFunctionFromTokenAndTypeArgs(UIntPtr moduleId, [In] int funcDef, UIntPtr classId, [In] uint cTypeArgs,
      ref UIntPtr typeArgs, out UIntPtr pFunctionId);

    void EnumModuleFrozenObjects(UIntPtr moduleId,
      [MarshalAs(UnmanagedType.Interface)] out ICorProfilerObjectEnum ppEnum);

    void GetArrayObjectInfo(UIntPtr objectId, [In] uint cDimensions, out uint pDimensionSizes,
      out int pDimensionLowerBounds, [Out] IntPtr ppData);

    void GetBoxClassLayout(UIntPtr classId, out uint pBufferOffset);

    void GetThreadAppDomain(UIntPtr threadId, out UIntPtr pAppDomainId);

    void GetRVAStaticAddress(UIntPtr classId, [In] int fieldToken, out IntPtr ppAddress);

    void GetAppDomainStaticAddress(UIntPtr classId, [In] int fieldToken, UIntPtr appDomainId, out IntPtr ppAddress);

    void GetThreadStaticAddress(UIntPtr classId, [In] int fieldToken, UIntPtr threadId, out IntPtr ppAddress);

    void GetContextStaticAddress(UIntPtr classId, [In] int fieldToken, UIntPtr contextId, out IntPtr ppAddress);

    void GetStaticFieldInfo(UIntPtr classId, [In] int fieldToken, out COR_PRF_STATIC_TYPE pFieldInfo);

    void GetGenerationBounds([In] uint cObjectRanges, out uint pcObjectRanges,
      [Out] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]
      COR_PRF_GC_GENERATION_RANGE[] ranges);

    void GetObjectGeneration(UIntPtr objectId, out COR_PRF_GC_GENERATION_RANGE range);

    void GetNotifiedExceptionClauseInfo(out COR_PRF_EX_CLAUSE_INFO pinfo);

  }

}