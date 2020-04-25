using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Interop {

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

}