#nullable enable
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Antijank.Debugging;
using Microsoft.Diagnostics.Runtime;
using Debug = Antijank.Debugging.Debug;

namespace Antijank {

  public class DebuggerContext : SynchronizationContext, IDisposable {

    private readonly Thread _thread;

    private static readonly BlockingCollection<(SendOrPostCallback Callback, object State, ManualResetEvent? Event)>
      ExecutionQueue = new BlockingCollection<(SendOrPostCallback, object, ManualResetEvent?)>
        (new ConcurrentQueue<(SendOrPostCallback, object, ManualResetEvent?)>());

    private DataTarget _attachedDt = null!;

    public DebuggerContext()
      => _thread = new Thread(DebuggerThreadAction) {
        Name = "Stack Scanner",
        IsBackground = true,
        Priority = ThreadPriority.Highest
      };

    public void Start()
      => _thread.Start();

    [ThreadStatic]
    public static DataTarget? InProcDataTarget;

    [ThreadStatic]
    public static ClrInfo? InProcClrInfo;

    [ThreadStatic]
    public static ClrRuntime? InProcClrRuntime;

    [ThreadStatic]
    private static CLRMetaHost? MetaHost;

    [ThreadStatic]
    private static CLRMetaHostPolicy? MetaHostPolicy;

    [ThreadStatic]
    private static CLRDebugging? Debugging;

    [ThreadStatic]
    private static ICorDebugProcess? DebugProcess;

    public static int ThreadId;

    private void DebuggerThreadAction() {
      ThreadId = Thread.CurrentThread.ManagedThreadId;

      var pid = -1;
      using (var proc = Process.GetCurrentProcess())
        pid = proc.Id;

      {
        MetaHost = MsCorEe.CLRCreateInstance<CLRMetaHost>();
        MetaHostPolicy = MsCorEe.CLRCreateInstance<CLRMetaHostPolicy>();
        Debugging = MsCorEe.CLRCreateInstance<CLRDebugging>();

        var version = RuntimeEnvironment.GetSystemVersion();
        var clrRt = MetaHost.GetRuntime(version, typeof(ICLRRuntimeInfo).GUID);
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

        var appDomCount = DebugProcess.EnumerateAppDomains().GetCount();

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
              var isDyn = mod.IsDynamic() ? "yes" : "no";
              Console.WriteLine($"Module: {modName} ({isDyn})");
              
              
            }

          }
        }
      }

      DataTarget inProcDt;
      ClrRuntime clrRt2;
      {
        _attachedDt = DataTarget.PassiveAttachToProcess(pid);
        inProcDt = new DataTarget(new InProcDataReader(_attachedDt.DataReader));
        var clrInfo = inProcDt.ClrVersions.Single();
        clrRt2 = clrInfo.CreateRuntime();

        InProcDataTarget = inProcDt;
        InProcClrInfo = clrInfo;
        InProcClrRuntime = clrRt2;
      }
      try {
        foreach (var (callback, state, waiter) in ExecutionQueue.GetConsumingEnumerable()) {
          callback(state);
          waiter?.Set();
        }
      }
      finally {
        _attachedDt.Dispose();
        inProcDt.Dispose();
        clrRt2.Dispose();
      }
    }

    public override void Send(SendOrPostCallback d, object state) {
      using var e = new ManualResetEvent(false);
      ExecutionQueue.Add((d, state, e));
      e.WaitOne();
    }

    public override void Post(SendOrPostCallback d, object state)
      => ExecutionQueue.Add((d, state, null));

    void IDisposable.Dispose() {
      ExecutionQueue.CompleteAdding();
      _thread.Join();
      foreach (var (_, _, waiter) in ExecutionQueue.GetConsumingEnumerable())
        waiter?.Dispose();
      ExecutionQueue.Dispose();
    }

  }

}