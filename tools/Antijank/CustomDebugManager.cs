using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using TaleWorlds.Library;
using static System.Reflection.BindingFlags;
using static Antijank.DebuggerContext;

namespace Antijank {

  public class CustomDebugManager : IDebugManager {

    public static readonly CustomDebugManager Instance = new CustomDebugManager();

    private static SynchronizationContext _syncCtx;

    private bool TestGenericMethod<T>() => typeof(T).IsPrimitive;

    static CustomDebugManager() {
      TaleWorlds.Library.Debug.DebugManager = Instance;

      var syncCtx = new DebuggerContext();
      _syncCtx = syncCtx;
      syncCtx.Start();
      SynchronizationContext.SetSynchronizationContext(syncCtx);

      _syncCtx?.Send(_ => {
        var thisType = typeof(CustomDebugManager);
        var testMethod = thisType.GetMethod(nameof(TestGenericMethod), Public | NonPublic | Static | BindingFlags.Instance);
        if (testMethod == null)
          throw new NotImplementedException();
      }, null);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Init() {
      // run static init
    }

    public void ShowWarning(string message)
      => Trace.TraceWarning(message);

    public void Assert(bool condition, string message,
      [CallerFilePath] string callerFile = "",
      [CallerMemberName] string callerMethod = "",
      [CallerLineNumber] int callerLine = 0) {
      if (!condition)
        Trace.TraceError(message);
    }

    public void Print(string message, int logLevel = 0, TaleWorlds.Library.Debug.DebugColor color = TaleWorlds.Library.Debug.DebugColor.White, ulong debugFilter = 17592186044416) {
      switch (logLevel) {
        default:
          Trace.TraceError($"LEVEL{logLevel}: {message}");
          break;
        case 1: // errors
          Trace.TraceError(message);
          break;
        case 2: // warnings
          Trace.TraceWarning(message);
          break;
        case 3: // info
          Trace.TraceInformation(message);
          break;
        case 4: // debug
          Trace.TraceInformation($"DEBUG: {message}");
          break;
      }
    }

    public void PrintError(string error, string stackTrace, ulong debugFilter = 17592186044416) {
      Trace.TraceError(error);
      Trace.TraceError(stackTrace);
    }

    public void PrintWarning(string warning, ulong debugFilter = 17592186044416)
      => Trace.TraceWarning(warning);

    public void DisplayDebugMessage(string message)
      => Trace.TraceInformation($"DEBUG: {message}");

    public void WatchVariable(string name, object value) {
    }

    public void BeginTelemetryScope(TelemetryLevelMask levelMask, string scopeName) {
    }

    public void EndTelemetryScope() {
    }

    public void WriteDebugLineOnScreen(string message)
      => Trace.TraceInformation($"DEBUG: {message}");

    public void RenderDebugLine(Vec3 position, Vec3 direction, uint color = 4294967295, bool depthCheck = false, float time = 0) {
    }

    public void RenderDebugSphere(Vec3 position, float radius, uint color = 4294967295, bool depthCheck = false, float time = 0) {
    }

    public void RenderDebugFrame(MatrixFrame frame, float lineLength, float time = 0) {
    }

    public void RenderDebugText(float screenX, float screenY, string text, uint color = 4294967295, float time = 0) {
    }

    public Vec3 GetDebugVector()
      => default;

    public void SetCrashReportCustomString(string customString)
      => Trace.TraceInformation($"Crash Report: {customString}");

    public void SetCrashReportCustomStack(string customStack)
      => Trace.TraceInformation($"Crash Report Stack Trace: {customStack}");

    public void Error(Exception exception, string message)
      => PrintError(message, exception.ToString());

  }

}