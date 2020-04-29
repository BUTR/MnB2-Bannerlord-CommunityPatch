using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace Antijank.Debugging {

  
  public class Debugger {

    private static readonly CLR_DEBUGGING_VERSION MaxClrDebuggingVersion = new CLR_DEBUGGING_VERSION {
      wMajor = ushort.MaxValue,
      wMinor = ushort.MaxValue,
      wBuild = ushort.MaxValue,
      wRevision = ushort.MaxValue
    };

    private static readonly Lazy<ICLRDebugging> _ClrDebugging
      = new Lazy<ICLRDebugging>(() => (ICLRDebugging)
        Debug.CLRCreateInstance(
          typeof(CLRDebuggingClass).GUID,
          typeof(ICLRDebugging).GUID
        ));

    private static readonly Lazy<ICorDebugDataTarget> InProcDataTarget
      = new Lazy<ICorDebugDataTarget>(() => new InProcCorDebugDataTarget());

    private static readonly Lazy<ICLRDebuggingLibraryProvider> InProcLibraryProvider
      = new Lazy<ICLRDebuggingLibraryProvider>(() => new InProcLibProvider());

    private static readonly string ClrModulePath;

    private static readonly IntPtr ClrBaseAddr
      = DebugHelpers.GetRuntimeModuleInfo(out ClrModulePath);

    private static readonly Lazy<ICorDebugProcess5> _CurrentDebugProcess
      = new Lazy<ICorDebugProcess5>(CurrentDebugProcessFactory);

    private static ICLRDebugging ClrDebugging
      => _ClrDebugging.Value;

    private static ICorDebugProcess5 CurrentDebugProcess
      => _CurrentDebugProcess.Value;

    public static void SubscribeToInProcessEvents() {
      var clrProc = CurrentDebugProcess;

      // ReSharper disable once SuspiciousTypeConversion.Global
      if (!(clrProc is ICorDebugProcess10 clrProc10))
        return;

      lock (clrProc)
        clrProc10.EnableGCNotificationEvents(true);
    }

    public static IEnumerable<COR_HEAPOBJECT> EnumerateHeap() {
      var clrProc = CurrentDebugProcess;

      lock (clrProc) {
        var heapInfo = clrProc.GetGCHeapInformation();

        if (!heapInfo.areGCStructuresValid)
          yield break;

        var gcHeapEnum = clrProc.EnumerateHeap();

        var objs = new COR_HEAPOBJECT[1];
        for (;;) {
          var hr = gcHeapEnum.Next(1, objs, out var fetched);

          if (hr == unchecked((int) 0x80070057)) // invalid arg
            continue; // can still continue if fetched anyway

          if (fetched < 1)
            break; // no more refs in sequence

          yield return objs[0];
        }

        Marshal.FinalReleaseComObject(gcHeapEnum);
      }
    }

    public static IEnumerable<(object Object, ICorDebugValue Location)>
      EnumerateHandles(CorGCReferenceType flags) {
      var clrProc = CurrentDebugProcess;

      lock (clrProc) {
        var heapRegions = GetHeapRegions();

        if (heapRegions.Count == 0)
          yield break;

        var gcHeapEnum = clrProc.EnumerateHandles(flags);

        var refs = new COR_GC_REFERENCE[1];
        for (;;) {
          var hr = gcHeapEnum.Next(1, refs, out var fetched);

          if (hr == unchecked((int) 0x80070057)) // invalid arg
            continue; // can still continue if fetched anyway

          if (fetched < 1)
            break; // no more refs in sequence

          var gcRef = refs[0];

          var gcLoc = gcRef.Location;

          if (gcLoc is null)
            continue;

          gcLoc.GetAddress(out var refAddr);

          if (refAddr == 0)
            continue;

          var objRef = Dereference(refAddr);

          if (objRef is null)
            continue;

          var tup = (objRef, gcLoc);

          yield return tup;

          tup.objRef = null;
          tup.gcLoc = null;

          Marshal.FinalReleaseComObject(gcLoc);
        }

        Marshal.FinalReleaseComObject(gcHeapEnum);
      }
    }

    public static IReadOnlyList<COR_SEGMENT> GetHeapRegions() {
      var clrProc = CurrentDebugProcess;

      lock (clrProc) {
        var heapInfo = clrProc.GetGCHeapInformation();

        if (!heapInfo.areGCStructuresValid)
          return null;

        var gcRegionEnum = clrProc.EnumerateHeapRegions();

        var regionCount = gcRegionEnum.GetCount();

        var regions = new COR_SEGMENT[regionCount];

        var hr = gcRegionEnum.Next(regionCount, regions, out var fetched);

        if (hr != 0)
          throw Marshal.GetExceptionForHR(hr);

        if (fetched != regionCount)
          throw new NotImplementedException($"Fetched {fetched} instead of {regionCount} heap regions.");

        Marshal.FinalReleaseComObject(gcRegionEnum);

        return regions;
      }
    }

    public static IEnumerable<HeapObjectInfo> EnumerateGcReferences(bool inclWeakRefs = false) {
      var clrProc = CurrentDebugProcess;

      lock (clrProc) {
        var heapRegions = GetHeapRegions();

        if (heapRegions.Count == 0)
          yield break;

        var gcRefEnum = clrProc.EnumerateGCReferences(inclWeakRefs);

        //gcRefEnum.Reset();
        var refs = new COR_GC_REFERENCE[1];

        for (;;) {
          refs[0] = default;

          var hr = gcRefEnum.Next(1, refs, out var fetched);

          // invalid arg
          if (hr == unchecked((int) 0x80070057)) {
            if (fetched < 1)
              continue; // can still continue if fetched anyway
          }

          if (fetched < 1)
            break; // no more refs in sequence

          var gcRef = refs[0];

          var gcRefType = gcRef.Type;

          var gcLoc = gcRef.Location;

          if (gcLoc is null)
            continue;

          gcLoc.GetAddress(out var refAddr);

          // null handle
          if (refAddr == 0) {
            Marshal.FinalReleaseComObject(gcLoc);
            continue;
          }

          var actualAddr = GetHeapDetail(refAddr, heapRegions, out var maybeHeapRegion);

          // also handle to null
          if (actualAddr == 0) {
            Marshal.FinalReleaseComObject(gcLoc);
            continue;
          }

          // ReSharper disable once SuspiciousTypeConversion.Global
          var size = (gcLoc as ICorDebugValue3)?.GetSize64() ?? gcLoc.GetSize();
          if (size == 0) {
            Marshal.FinalReleaseComObject(gcLoc);
            continue;
          }

          var elemType = gcLoc.GetType();

          if (maybeHeapRegion == null) {
            // wait what? something not on the heap?
            //System.Diagnostics.Debugger.Break();

            if (elemType == CorElementType.ELEMENT_TYPE_CLASS) {
              var exactType = (gcLoc as ICorDebugValue2).GetExactType();
              var objClass = exactType.GetClass();
              var objClassMod = objClass.GetModule();
              var objClassToken = objClass.GetToken();
              var objMdToken = unchecked((int) objClassToken);
              var objModAddr = (IntPtr) unchecked((long) ((UIntPtr) objClassMod.GetBaseAddress()).ToUInt64());
              var fth = new FastTypeHandle(objModAddr, objMdToken);
              var t = fth.ResolveType();

              Marshal.FinalReleaseComObject(objClassMod);
              Marshal.FinalReleaseComObject(objClass);
              Marshal.FinalReleaseComObject(exactType);

              if (t.MetadataToken != objMdToken) {
                yield return new HeapObjectInfo(null, elemType, size, uint.MaxValue, ulong.MaxValue, CorDebugGenerationTypes.Invalid);

                Marshal.FinalReleaseComObject(gcLoc);
                continue;
              }

              // I guess we're OK...
            }
            else {
              yield return new HeapObjectInfo(null, elemType, size, uint.MaxValue, ulong.MaxValue, CorDebugGenerationTypes.Invalid);

              Marshal.FinalReleaseComObject(gcLoc);
              continue;
            }
          }

          var heapRegion = maybeHeapRegion ?? COR_SEGMENT.Invalid;

          switch (gcRefType) {
            default:
              continue;
            case CorGCReferenceType.CorReferenceStack:
            case CorGCReferenceType.CorHandleStrong:
            case CorGCReferenceType.CorHandleStrongAsyncPinned:
            case CorGCReferenceType.CorHandleStrongPinning:
            case CorGCReferenceType.CorHandleStrongDependent:
            case CorGCReferenceType.CorHandleStrongRefCount:
              break;
            case CorGCReferenceType.CorHandleWeakShort:
            case CorGCReferenceType.CorHandleWeakLong:
            case CorGCReferenceType.CorHandleWeakRefCount:
              if (inclWeakRefs)
                break;

              Marshal.FinalReleaseComObject(gcLoc);
              continue;
          }

          var objRef = Dereference(refAddr);

          if (objRef is null) {
            Marshal.FinalReleaseComObject(gcLoc);
            continue;
          }

          yield return new HeapObjectInfo(objRef, elemType, size, heapRegion.heap, heapRegion.start, heapRegion.type);

          Marshal.FinalReleaseComObject(gcLoc);
        }

        Marshal.FinalReleaseComObject(gcRefEnum);
      }
    }

    private static unsafe RuntimeTypeHandle? GetInProcTypeHandle(COR_TYPEID typeId) {
      if (!typeId.IsMethodTable())
        return null;

      var methodTableAddress = typeId.token1;
      // RuntimeTypeHandle structs contain a RuntimeType class that contains the MT even though their "value" is just this

      throw new NotImplementedException();
    }

    private static unsafe ulong GetHeapDetail(
      ulong refAddr,
      IReadOnlyList<COR_SEGMENT> heapRegions,
      out COR_SEGMENT? heapSeg
    ) {
      var actualAddr = (*(UIntPtr*) refAddr).ToUInt64();

      if (actualAddr == 0) {
        heapSeg = null;
        return actualAddr;
      }

      foreach (var heapRegion in heapRegions) {
        if (actualAddr < heapRegion.start
          || actualAddr >= heapRegion.end)
          continue;

        heapSeg = heapRegion;
        return actualAddr;
      }

      heapSeg = null;
      return actualAddr;
    }

    private static unsafe object Dereference(ulong refAddr)
      => Unsafe.AsRef<object>(((IntPtr) refAddr).ToPointer());

    private static unsafe T Dereference<T>(ulong refAddr) where T : class
      => Unsafe.AsRef<T>(((IntPtr) refAddr).ToPointer());

    public static IReadOnlyDictionary<ulong, IReadOnlyDictionary<Type, (ulong Bytes, ulong Count)>> CreateReferenceReport(ICollection<COR_SEGMENT> collectedHeapRegions, bool inclWeakRefs = false) {
      var reports = ImmutableDictionary
        .CreateBuilder<ulong, IReadOnlyDictionary<Type, (ulong Bytes, ulong Count)>>();

      reports.Add(ulong.MaxValue, ImmutableDictionary.CreateBuilder<Type, (ulong Bytes, ulong Count)>());

      // ReSharper disable once TooWideLocalVariableScope
      foreach (var objHeapInfo in EnumerateGcReferences(inclWeakRefs)) {
        var objRef = objHeapInfo.Value;
        var objType = objRef?.GetType();

        if (objType == null) {
          switch (objHeapInfo.ElementType) {
            case CorElementType.ELEMENT_TYPE_SZARRAY:
              objType = typeof(Array);
              break;
            default:
              objType = typeof(void);
              break;
          }
        }

        var regionStart = objHeapInfo.HeapBaseAddress;
        if (!reports.TryGetValue(regionStart, out var report)) {
          reports.Add(regionStart, report = ImmutableDictionary.CreateBuilder<Type, (ulong Bytes, ulong Count)>());
          if (collectedHeapRegions.All(r => r.start != regionStart)) {
            var heapRegions = GetHeapRegions();
            foreach (var heapRegion in heapRegions) {
              if (collectedHeapRegions.All(r => r.start != heapRegion.start))
                collectedHeapRegions.Add(heapRegion);
            }
          }
        }

        if (!report.TryGetValue(objType, out var statsBox))
          ((ImmutableDictionary<Type, (ulong Bytes, ulong Count)>.Builder) report)
            .Add(objType, statsBox = (0uL, 0uL));

        // ReSharper disable once UseDeconstruction
        ref var statsTup = ref statsBox;

        statsTup.Bytes += objHeapInfo.Size;
        ++statsTup.Count;
      }

      foreach (var reportKey in reports.Keys.ToArray())
        reports[reportKey] = ((ImmutableDictionary<Type, (ulong Bytes, ulong Count)>.Builder) reports[reportKey])
          .ToImmutable();

      return reports.ToImmutable();
    }

    public static string ParseReferenceReport(IReadOnlyDictionary<ulong, IReadOnlyDictionary<Type, (ulong Bytes, ulong Count)>> heapReports, ICollection<COR_SEGMENT> collectedHeapRegions) {
      using var sw = new StringWriter(CultureInfo.InvariantCulture);

      ParseReferenceReport(sw, heapReports, collectedHeapRegions);

      return sw.ToString();
    }

    private static void ParseReferenceReport(
      TextWriter output,
      IReadOnlyDictionary<ulong, IReadOnlyDictionary<Type, (ulong Bytes, ulong Count)>> heapReports,
      ICollection<COR_SEGMENT> collectedHeapRegions
    ) {
      ulong totalCount = 0;
      ulong totalBytes = 0;

      var currentHeapRegions = GetHeapRegions();

      foreach (var heapRegion in currentHeapRegions)
        if (collectedHeapRegions.All(r => r.start != heapRegion.start))
          collectedHeapRegions.Add(heapRegion);

      output.WriteLine($"- {"Instance Count",20} {"Data Bytes",20}  - Instance Assembly Qualified Type Name or Heap Region Description");

      foreach (var heapRegion in collectedHeapRegions) {
        output.WriteLine();

        var isCurrent = currentHeapRegions.Any(r => r.start == heapRegion.start);
        var regionStart = heapRegion.start;
        var regionSize = heapRegion.end - heapRegion.start;
        var regionSizeKb = (regionSize / 1024.0).ToString("F2");
        if (!heapReports.TryGetValue(regionStart, out var heapReport)) {
          output.WriteLine(
            $"- {0,20} {0,20}B - Heap {heapRegion.heap} {heapRegion.type} {(isCurrent ? regionSize == 0 ? "Placeholder" : "Reserved" : "Historical")} Region ({regionSizeKb}KB at 0x{heapRegion.start:x8})");
          continue;
        }

        ulong heapRegionCount = 0;
        ulong heapRegionBytes = 0;

        foreach (var type in heapReport.Keys) {
          var aqn = type.AssemblyQualifiedName;

          var statsBox = heapReport[type];

          // ReSharper disable once UseDeconstruction
          ref var statsTup = ref statsBox;

          heapRegionCount += statsTup.Count;
          heapRegionBytes += statsTup.Bytes;

          output.WriteLine($"  {statsTup.Count,20} {statsTup.Bytes,20}B   {aqn}");
        }

        output.WriteLine(
          $"- {heapRegionCount,20} {heapRegionBytes,20}B - Heap {heapRegion.heap} {heapRegion.type} {(isCurrent ? "Active" : "Moved")} Region ({regionSizeKb}KB at 0x{heapRegion.start:x8})");

        totalCount += heapRegionCount;
        totalBytes += heapRegionBytes;
      }

      if (heapReports.ContainsKey(uint.MaxValue)) {
        output.WriteLine();

        var heapReport = heapReports[uint.MaxValue];
        ulong heapRegionCount = 0;
        ulong heapRegionBytes = 0;

        foreach (var type in heapReport.Keys) {
          var aqn = type == typeof(void) ? "(???)" : type.AssemblyQualifiedName;

          var statsBox = heapReport[type];

          // ReSharper disable once UseDeconstruction
          ref var statsTup = ref statsBox;

          heapRegionCount += statsTup.Count;
          heapRegionBytes += statsTup.Bytes;

          output.WriteLine($"  {statsTup.Count,20} {statsTup.Bytes,20}B   {aqn}");
        }

        output.WriteLine(
          $"- {heapRegionCount,20} {heapRegionBytes,20}B - Non-Heap");

        totalCount += heapRegionCount;
        totalBytes += heapRegionBytes;
      }

      output.WriteLine();
      output.WriteLine($"- {totalCount,20} {totalBytes,20}B - Grand Total");
    }

    private static ICorDebugProcess DebugProcess(int pid)
      => throw new NotImplementedException();

    private static ICorDebugProcess5 CurrentDebugProcessFactory() {
      var clrBaseAddr = DebugHelpers.GetRuntimeModuleInfo(out var clrModString);

      var clrBaseAddrUnsigned = unchecked((ulong) clrBaseAddr.ToInt64());

      var clrDebuggingVersion = new CLR_DEBUGGING_VERSION();

      ClrDebugging.OpenVirtualProcess(
        clrBaseAddrUnsigned,
        InProcDataTarget.Value,
        InProcLibraryProvider.Value,
        MaxClrDebuggingVersion,
        typeof(ICorDebugProcess5).GUID,
        out var clrProcObj,
        clrDebuggingVersion,
        out _
      );

      var clrProc = (ICorDebugProcess5) clrProcObj;
      return clrProc;
    }

  }

}