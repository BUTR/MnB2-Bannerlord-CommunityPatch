using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  
  [ComImport, Guid("BADB5F70-58DA-43a9-A1C6-D74819F19B15"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  public interface IMetaDataTables2 {

    int GetMetaDataStorage(out IntPtr ppvMd, out uint pcbMd);

    int GetMetaDataStreamInfo(uint ix, [MarshalAs(UnmanagedType.LPArray)] char[] ppchName, out IntPtr ppv, out uint pcb);

  }

}