using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using JetBrains.Annotations;

namespace Com {

  // STGM from objbase.h
  // https://docs.microsoft.com/en-us/windows/win32/stg/stgm-constants
  [PublicAPI]
  [Flags]
  public enum StorageMode : int {

    // Access
    Read = 0,

    Write = 1 << 0,

    ReadWrite = 1 << 1,

    // Sharing

    ShareExclusive = 1 << 4,

    ShareDenyWrite = 1 << 5,

    ShareDenyRead = ShareExclusive | ShareDenyWrite,

    ShareDenyNone = 1 << 6,

    Priority = 1 << 18,

    // Creation

    FailIfThere = 0,

    Create = 1 << 12,

    Convert = 1 << 17,

    // Transactioning

    Direct = 0,

    Transacted = 1 << 16,

    // Transactioning Performance

    NoScratch = 1 << 20,

    NoSnapshot = 1 << 21,

    // Other

    DirectSingleWriterMultipleReader = 1 << 22,

    Simple = 1 << 27,

    // Delete On Release

    DeleteOnRelease = 1 << 26,

  }

  // https://docs.microsoft.com/en-us/previous-versions/windows/internet-explorer/ie-developer/platform-apis/ms537175(v%3Dvs.85)
  [PublicAPI]
  public enum UrlZone {

    Invalid = -1,

    LocalMachine = 0,

    Intranet,

    Trusted,

    Internet,

    Untrusted

  }

  // UrlMon.Idl
  [PublicAPI]
  [ComImport]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("CD45F185-1B21-48E2-967B-EAD743A8914E")]
  public interface IZoneIdentifier {

    UrlZone GetId();

    void SetId(UrlZone id);

    void Remove();

  }

  [ComImport]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("CD45F185-1B21-48E2-967B-EAD743A8914E")]
  public interface IZoneIdentifier2 {

    string GetLastWriterPackageFamilyName();

    void SetLastWriterPackageFamilyName(string packageFamilyName);

    void RemoveLastWriterPackageFamilyName();

    uint GetAppZoneId();

    void SetAppZoneId(uint zone);

    void RemoveAppZoneId();

  }

  // https://docs.microsoft.com/en-us/previous-versions/windows/internet-explorer/ie-developer/platform-apis/ms537029(v=vs.85)?redirectedfrom=MSDN
  [PublicAPI]
  [ClassInterface(ClassInterfaceType.None)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  [Guid("0968E258-16C7-4DBA-AA86-462DD61E31A3")] // CLSID_PersistentZoneIdentifier
  public class PersistentZoneIdentifier : IPersistFile, IZoneIdentifier {

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void GetClassID(out Guid pClassId);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void GetCurFile(out string fileName);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern int IsDirty();

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void Load(string fileName, int storageMode);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void Save(string fileName, bool remember);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void SaveCompleted(string fileName);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern UrlZone GetId();

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void Remove();

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void SetId(UrlZone id);

  }

  [PublicAPI]
  public static class PersistentZoneIdentifierExtensions {

    public static void Load(this PersistentZoneIdentifier pzi, string fileName, StorageMode storageMode)
      => pzi.Load(fileName, (int) storageMode);

  }

}