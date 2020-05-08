#nullable enable
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Antijank.Debugging;
using static System.Reflection.BindingFlags;

namespace Antijank {

  public class DebuggerContext : SynchronizationContext, IDisposable {

    public new static DebuggerContext Current { get; }

    static DebuggerContext() {
      var syncCtx = new DebuggerContext();
      Current = syncCtx;
      syncCtx.Start();
    }

    public static void Init() {
      // run static initializer
    }

    private readonly Thread _thread;

    private readonly BlockingCollection<Action>
      _executionQueue = new BlockingCollection<Action>
        (new ConcurrentQueue<Action>());

    [DllImport("AntijankProfiler")]
    [return: MarshalAs(UnmanagedType.Interface)]
    private static extern object GetCorProfilerInfo();

#pragma warning disable 649
    internal readonly ref struct InjectionMetadata {

      public readonly int Injection;

      public readonly int Dispatch;

      public readonly int Pack1, Pack2, Pack3, Pack4, Pack5, Pack6, Pack7, Pack8, Pack9, Pack10, Pack11, Pack12;

      public readonly int GenVar1, GenVar2, GenVar3, GenVar4, GenVar5, GenVar6, GenVar7, GenVar8, GenVar9, GenVar10, GenVar11, GenVar12;

      public readonly int GenPack1, GenPack2, GenPack3, GenPack4, GenPack5, GenPack6, GenPack7, GenPack8, GenPack9, GenPack10, GenPack11, GenPack12;

      public readonly int GenPackSpec1, GenPackSpec2, GenPackSpec3, GenPackSpec4, GenPackSpec5, GenPackSpec6, GenPackSpec7, GenPackSpec8, GenPackSpec9, GenPackSpec10, GenPackSpec11, GenPackSpec12;

      public int GetPack(int i)
        => i < 0 || i >= 12
          ? throw new ArgumentOutOfRangeException(nameof(i))
          : Unsafe.Add(ref Unsafe.AsRef(Pack1), i);

      public int GetGenVar(int i)
        => i < 0 || i >= 12
          ? throw new ArgumentOutOfRangeException(nameof(i))
          : Unsafe.Add(ref Unsafe.AsRef(GenVar1), i);

      public int GetGenPack(int i)
        => i < 0 || i >= 12
          ? throw new ArgumentOutOfRangeException(nameof(i))
          : Unsafe.Add(ref Unsafe.AsRef(GenPack1), i);

      public int GetGenPackSpec(int i)
        => i < 0 || i >= 12
          ? throw new ArgumentOutOfRangeException(nameof(i))
          : Unsafe.Add(ref Unsafe.AsRef(GenPackSpec1), i);

    }
#pragma warning restore 649

    [DllImport("AntijankProfiler")]
    internal static extern unsafe bool GetInjections(
      [MarshalAs(UnmanagedType.LPWStr)] [In] string wszModule,
      [Out] out InjectionMetadata* injections);

    private DebuggerContext()
      => _thread = new Thread(DebuggerThreadAction) {
        Name = "Debugger Thread",
        IsBackground = true,
        Priority = ThreadPriority.Highest
      };

    private void Start()
      => _thread.Start();

    [ThreadStatic]
    public static CLRMetaHost? MetaHost;

    [ThreadStatic]
    public static CLRMetaHostPolicy? MetaHostPolicy;

    [ThreadStatic]
    public static CLRDebugging? Debugging;

    [ThreadStatic]
    public static ICorDebugProcess? DebugProcess;

    [ThreadStatic]
    public static ICLRRuntimeInfo ClrRuntime;

    [ThreadStatic]
    public static ICorProfilerInfo? ProfilerInfo;

    public static int ThreadId;

    public static readonly MethodInfo TestEnCMethod = typeof(DebuggerContext).GetMethod(nameof(TestEnC), DeclaredOnly | Public | Static);

    public static bool TestEnC() => false;

    private unsafe void DebuggerThreadAction() {
      SetSynchronizationContext(this);

      ThreadId = Thread.CurrentThread.ManagedThreadId;

      ProfilerInfo = GetCorProfilerInfo() as ICorProfilerInfo;

      var ownAsm = typeof(DebuggerContext).Assembly;
      var ownAsmPath = new Uri(ownAsm.CodeBase).LocalPath;

      var pid = -1;
      using (var proc = Process.GetCurrentProcess())
        pid = proc.Id;

      {
        MetaHost = MsCorEe.CLRCreateInstance<CLRMetaHost>();
        MetaHostPolicy = MsCorEe.CLRCreateInstance<CLRMetaHostPolicy>();
        Debugging = MsCorEe.CLRCreateInstance<CLRDebugging>();

        var version = RuntimeEnvironment.GetSystemVersion();
        ClrRuntime = MetaHost.GetRuntime(version, typeof(ICLRRuntimeInfo).GUID);

        if (ProfilerInfo != null) {
          // ok
        }
        /*
        var mddx = (IMetaDataDispenserEx) clrRt.GetInterface(
          typeof(CorMetaDataDispenserClass).GUID,
          typeof(IMetaDataDispenserEx).GUID
        );

        var encMode = (CorSetEnC) (uint) mddx.GetOption(MetaDataDispenserOptions.MetaDataSetENC);

        if (encMode != CorSetEnC.MDUpdateFull)
          throw new NotImplementedException(encMode.ToString());
        */

        /*
        profilerClsid = typeof(ProfilerImpl).GUID;
        profiler.AttachProfiler((uint) pid, 10,
          ref profilerClsid, ownAsmPath, default, 0);
          */

        var dt = new InProcCorDebugDataTarget();
        //clrRt.GetInterface()
        var lp = new InProcLibProvider();
        var maxVersion = new CLR_DEBUGGING_VERSION {wMajor = 9999, wMinor = 9999, wBuild = 9999, wRevision = 9999};
        CLR_DEBUGGING_VERSION gotVersion = default;

        var clrAddr = lp.FindLoadedLibrary("clr.dll");

        Debugging.OpenVirtualProcess(
          unchecked((ulong) clrAddr.ToInt64()),
          dt,
          lp,
          ref maxVersion,
          typeof(ICorDebugProcess).GUID,
          out object procObj,
          ref gotVersion,
          out var flags
        );

        DebugProcess = (ICorDebugProcess) procObj;

        /*
        var sbName = new StringBuilder(4096);
        foreach (var appDom in DebugHelpers.GetEnumerable<ICorDebugAppDomain>(DebugProcess.EnumerateAppDomains().Next)) {
          var appDomId = appDom.GetID();
          var sb = new StringBuilder(4096);

          var appDomName = DebugHelpers.GetString(appDom.GetName);
          Console.WriteLine($"AppDomain: {appDomId} {appDomName}");

          foreach (var asm in DebugHelpers.GetEnumerable<ICorDebugAssembly>(appDom.EnumerateAssemblies().Next)) {
            var asmName = DebugHelpers.GetString(asm.GetName);
            Console.WriteLine($"Assembly: {asmName}");

            foreach (var mod in DebugHelpers.GetEnumerable<ICorDebugModule>(asm.EnumerateModules().Next)) {
              var modName = DebugHelpers.GetString(mod.GetName);
              var modType = mod.IsDynamic() ? "Dynamic" : "Static";
              Console.WriteLine($"{modType} Module: {modName}");
              if (!modName.Equals(ownAsmPath, StringComparison.OrdinalIgnoreCase))
                continue;
            }
          }
        }
        */
      }

      foreach (var callback in _executionQueue.GetConsumingEnumerable()) {
        try {
          callback();
        }
        catch (Exception ex) {
          Trace.TraceError(ex.ToString());
        }
      }
    }

    public static ulong FindModuleAddressAndSize(string module, out uint size) {
      var mod = FindModule(module);
      if (mod == null) {
        size = 0;
        return 0;
      }

      var addr = mod.GetBaseAddress();
      size = mod.GetSize();
      //Console.WriteLine($"0x{addr:X16} {size,10}b: {modName}");
      return addr;
    }

    public static ICorDebugModule? FindModule(string module) {
      if (DebugProcess == null)
        throw new InvalidOperationException();

      var appDomEnum = DebugProcess.EnumerateAppDomains();

      foreach (var appDom in DebugHelpers.GetEnumerable<ICorDebugAppDomain>(appDomEnum.Next)) {
        var asmEnum = appDom.EnumerateAssemblies();
        foreach (var asm in DebugHelpers.GetEnumerable<ICorDebugAssembly>(asmEnum.Next)) {
          var modsEnum = asm.EnumerateModules();
          foreach (var mod in DebugHelpers.GetEnumerable<ICorDebugModule>(modsEnum.Next)) {
            var sb = new StringBuilder(512);
            mod.GetName((uint) sb.Capacity, out var len, sb);
            var modName = sb.ToString(0, (int) len - 1);
            if (!module.Equals(modName, StringComparison.OrdinalIgnoreCase))
              continue;

            return mod;
          }
        }
      }

      return null;
    }

    public static string? GetModuleAtAddress(ulong address) {
      if (DebugProcess == null)
        throw new InvalidOperationException();

      var appDomEnum = DebugProcess.EnumerateAppDomains();

      foreach (var appDom in DebugHelpers.GetEnumerable<ICorDebugAppDomain>(appDomEnum.Next)) {
        var asmEnum = appDom.EnumerateAssemblies();
        foreach (var asm in DebugHelpers.GetEnumerable<ICorDebugAssembly>(asmEnum.Next)) {
          var modsEnum = asm.EnumerateModules();
          foreach (var mod in DebugHelpers.GetEnumerable<ICorDebugModule>(modsEnum.Next)) {
            var addr = mod.GetBaseAddress();

            if (addr != address)
              continue;

            var sb = new StringBuilder(512);
            mod.GetName((uint) sb.Capacity, out var len, sb);
            return sb.ToString(0, (int) len - 1);
          }
        }
      }

      return null;
    }

    public override void Send(SendOrPostCallback d, object state) {
      if (_thread == Thread.CurrentThread) {
        d(state);
        return;
      }

      using var e = new ManualResetEvent(false);

      void SentAction() {
        try {
          d(state);
        }
        finally {
          // ReSharper disable once AccessToDisposedClosure
          e?.Set();
        }
      }

      _executionQueue.Add(SentAction);
      e.WaitOne();
    }

    public void Send(Action d) {
      if (_thread == Thread.CurrentThread) {
        d();
        return;
      }

      using var e = new ManualResetEvent(false);

      void SentAction() {
        try {
          d();
        }
        finally {
          // ReSharper disable once AccessToDisposedClosure
          e?.Set();
        }
      }

      _executionQueue.Add(SentAction);
      e.WaitOne();
    }

    public override void Post(SendOrPostCallback d, object state)
      => _executionQueue.Add(() => d(state));

    public void Post(Action d)
      => _executionQueue.Add(d);

    void IDisposable.Dispose() {
      _executionQueue.CompleteAdding();
      _thread.Join();
      _executionQueue.Dispose();
    }

  }

}