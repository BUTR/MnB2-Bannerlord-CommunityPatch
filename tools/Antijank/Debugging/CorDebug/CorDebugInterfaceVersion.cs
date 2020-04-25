using JetBrains.Annotations;

namespace Antijank.Debugging {

  [PublicAPI]
  public enum CorDebugInterfaceVersion {

    CorDebugInvalidVersion = 0,

    CorDebugVersion_1_0 = CorDebugInvalidVersion + 1,

    ver_ICorDebugManagedCallback = CorDebugVersion_1_0,

    ver_ICorDebugUnmanagedCallback = CorDebugVersion_1_0,

    ver_ICorDebug = CorDebugVersion_1_0,

    ver_ICorDebugController = CorDebugVersion_1_0,

    ver_ICorDebugAppDomain = CorDebugVersion_1_0,

    ver_ICorDebugAssembly = CorDebugVersion_1_0,

    ver_ICorDebugProcess = CorDebugVersion_1_0,

    ver_ICorDebugBreakpoint = CorDebugVersion_1_0,

    ver_ICorDebugFunctionBreakpoint = CorDebugVersion_1_0,

    ver_ICorDebugModuleBreakpoint = CorDebugVersion_1_0,

    ver_ICorDebugValueBreakpoint = CorDebugVersion_1_0,

    ver_ICorDebugStepper = CorDebugVersion_1_0,

    ver_ICorDebugRegisterSet = CorDebugVersion_1_0,

    ver_ICorDebugThread = CorDebugVersion_1_0,

    ver_ICorDebugChain = CorDebugVersion_1_0,

    ver_ICorDebugFrame = CorDebugVersion_1_0,

    ver_ICorDebugILFrame = CorDebugVersion_1_0,

    ver_ICorDebugNativeFrame = CorDebugVersion_1_0,

    ver_ICorDebugModule = CorDebugVersion_1_0,

    ver_ICorDebugFunction = CorDebugVersion_1_0,

    ver_ICorDebugCode = CorDebugVersion_1_0,

    ver_ICorDebugClass = CorDebugVersion_1_0,

    ver_ICorDebugEval = CorDebugVersion_1_0,

    ver_ICorDebugValue = CorDebugVersion_1_0,

    ver_ICorDebugGenericValue = CorDebugVersion_1_0,

    ver_ICorDebugReferenceValue = CorDebugVersion_1_0,

    ver_ICorDebugHeapValue = CorDebugVersion_1_0,

    ver_ICorDebugObjectValue = CorDebugVersion_1_0,

    ver_ICorDebugBoxValue = CorDebugVersion_1_0,

    ver_ICorDebugStringValue = CorDebugVersion_1_0,

    ver_ICorDebugArrayValue = CorDebugVersion_1_0,

    ver_ICorDebugContext = CorDebugVersion_1_0,

    ver_ICorDebugEnum = CorDebugVersion_1_0,

    ver_ICorDebugObjectEnum = CorDebugVersion_1_0,

    ver_ICorDebugBreakpointEnum = CorDebugVersion_1_0,

    ver_ICorDebugStepperEnum = CorDebugVersion_1_0,

    ver_ICorDebugProcessEnum = CorDebugVersion_1_0,

    ver_ICorDebugThreadEnum = CorDebugVersion_1_0,

    ver_ICorDebugFrameEnum = CorDebugVersion_1_0,

    ver_ICorDebugChainEnum = CorDebugVersion_1_0,

    ver_ICorDebugModuleEnum = CorDebugVersion_1_0,

    ver_ICorDebugValueEnum = CorDebugVersion_1_0,

    ver_ICorDebugCodeEnum = CorDebugVersion_1_0,

    ver_ICorDebugTypeEnum = CorDebugVersion_1_0,

    ver_ICorDebugErrorInfoEnum = CorDebugVersion_1_0,

    ver_ICorDebugAppDomainEnum = CorDebugVersion_1_0,

    ver_ICorDebugAssemblyEnum = CorDebugVersion_1_0,

    ver_ICorDebugEditAndContinueErrorInfo = CorDebugVersion_1_0,

    ver_ICorDebugEditAndContinueSnapshot = CorDebugVersion_1_0,

    CorDebugVersion_1_1 = CorDebugVersion_1_0 + 1,
    // no interface definitions in v1.1

    CorDebugVersion_2_0 = CorDebugVersion_1_1 + 1,

    ver_ICorDebugManagedCallback2 = CorDebugVersion_2_0,

    ver_ICorDebugAppDomain2 = CorDebugVersion_2_0,

    ver_ICorDebugAssembly2 = CorDebugVersion_2_0,

    ver_ICorDebugProcess2 = CorDebugVersion_2_0,

    ver_ICorDebugStepper2 = CorDebugVersion_2_0,

    ver_ICorDebugRegisterSet2 = CorDebugVersion_2_0,

    ver_ICorDebugThread2 = CorDebugVersion_2_0,

    ver_ICorDebugILFrame2 = CorDebugVersion_2_0,

    ver_ICorDebugInternalFrame = CorDebugVersion_2_0,

    ver_ICorDebugModule2 = CorDebugVersion_2_0,

    ver_ICorDebugFunction2 = CorDebugVersion_2_0,

    ver_ICorDebugCode2 = CorDebugVersion_2_0,

    ver_ICorDebugClass2 = CorDebugVersion_2_0,

    ver_ICorDebugValue2 = CorDebugVersion_2_0,

    ver_ICorDebugEval2 = CorDebugVersion_2_0,

    ver_ICorDebugObjectValue2 = CorDebugVersion_2_0,

    // CLR v4 - next major CLR version after CLR v2
    // Includes Silverlight 4
    CorDebugVersion_4_0 = CorDebugVersion_2_0 + 1,

    ver_ICorDebugThread3 = CorDebugVersion_4_0,

    ver_ICorDebugThread4 = CorDebugVersion_4_0,

    ver_ICorDebugStackWalk = CorDebugVersion_4_0,

    ver_ICorDebugNativeFrame2 = CorDebugVersion_4_0,

    ver_ICorDebugInternalFrame2 = CorDebugVersion_4_0,

    ver_ICorDebugRuntimeUnwindableFrame = CorDebugVersion_4_0,

    ver_ICorDebugHeapValue3 = CorDebugVersion_4_0,

    ver_ICorDebugBlockingObjectEnum = CorDebugVersion_4_0,

    ver_ICorDebugValue3 = CorDebugVersion_4_0,

    CorDebugVersion_4_5 = CorDebugVersion_4_0 + 1,

    ver_ICorDebugComObjectValue = CorDebugVersion_4_5,

    ver_ICorDebugAppDomain3 = CorDebugVersion_4_5,

    ver_ICorDebugCode3 = CorDebugVersion_4_5,

    ver_ICorDebugILFrame3 = CorDebugVersion_4_5,

    CorDebugLatestVersion = CorDebugVersion_4_5

  }

}