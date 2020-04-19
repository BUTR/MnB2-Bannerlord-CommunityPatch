using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Diagnostics.Runtime;
using TaleWorlds.Library;

namespace Antijank {

  public class CustomDebugManager : IDebugManager {

    private static readonly CustomDebugManager Instance = new CustomDebugManager();

    private static DataTarget _attachedDt;

    private static DataTarget _inProcDt;

    private static ClrRuntime _debugRt;

    private static ClrInfo _clrInfo;

    static CustomDebugManager() {
      TaleWorlds.Library.Debug.DebugManager = Instance;
      
      
      
      var pid = -1;
      using (var proc = Process.GetCurrentProcess())
        pid = proc.Id;

      _attachedDt = DataTarget.PassiveAttachToProcess(pid);
      _inProcDt = new DataTarget(new InProcDataReader(_attachedDt.DataReader));

      _clrInfo = _inProcDt.ClrVersions.Single();
      _debugRt = _clrInfo.CreateRuntime();
      
      /*
      foreach (var thread in _debugRt.Threads) {
        if (thread.IsUnstarted)
          continue;

        var i = 0;
        foreach (var frame in thread.EnumerateStackTrace()) {
          if (i++ < 100)
            continue;

          Debugger.Break();
          break;
        }
      }
      */

    }

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

  }

}