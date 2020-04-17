using System.Diagnostics;
using System.Runtime.CompilerServices;
using TaleWorlds.Library;

namespace FixedAssemblyResolution {

  public class CustomDebugManager : IDebugManager {

    private static readonly CustomDebugManager Instance = new CustomDebugManager();

    static CustomDebugManager()
      => TaleWorlds.Library.Debug.DebugManager = Instance;

    public static void Init() {
      // run static init
    }

    public void ShowWarning(string message)
      => Trace.TraceWarning(message);

    public void Assert(bool condition, string message, [CallerFilePath] string CallerFile = "", [CallerMemberName] string CallerMethod = "", [CallerLineNumber] int CallerLine = 0) {
      if (!condition)
        Trace.TraceError(message);
    }

    public void Print(string message, int logLevel = 0, TaleWorlds.Library.Debug.DebugColor color = TaleWorlds.Library.Debug.DebugColor.White, ulong debugFilter = 17592186044416) {
      switch (logLevel) {
        default:
          Trace.TraceError($"({logLevel}) {message}");
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