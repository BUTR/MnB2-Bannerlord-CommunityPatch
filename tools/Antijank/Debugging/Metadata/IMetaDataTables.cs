using System;
using System.Runtime.InteropServices;

namespace Antijank.Debugging {

  [ComImport, Guid("D8F579AB-402D-4b8e-82D9-5D63B1065C68"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  public interface IMetaDataTables {

    int GetStringHeapSize(out uint pcbStrings);

    int GetBlobHeapSize(out uint pcbBlobs);

    int GetGuidHeapSize(out uint pcbGuids);

    int GetUserStringHeapSize(out uint pcbBlobs);

    int GetNumTables(out uint pcTables);

    int GetTableIndex(uint token, out uint pixTbl);

    int GetTableInfo(uint ixTbl, out uint pcbRow, out uint pcRows, out uint pcCols, out uint piKey, [MarshalAs(UnmanagedType.LPArray)] char[] ppName);

    int GetColumnInfo(uint ixTbl, uint ixCol, out uint poCol, out uint pcbCol, out uint pType, [MarshalAs(UnmanagedType.LPArray)] char[] ppName);

    int GetCodedTokenInfo(uint ixCdTkn, out uint pcTokens, [MarshalAs(UnmanagedType.LPArray)] uint[] ppTokens, [MarshalAs(UnmanagedType.LPArray)] char[] ppName);

    int GetRow(uint ixTbl, uint rid, out IntPtr ppRow);

    int GetColumn(uint ixTbl, uint ixCol, uint rid, out uint pVal);

    int GetString(uint ixString, [MarshalAs(UnmanagedType.LPArray)] char[] ppString);

    int GetBlob(uint ixBlob, out uint pcbData, out IntPtr ppData);

    int GetGuid(uint ixGuid, out Guid ppGUID);

    int GetUserString(uint ixUserString, out uint pcbData, out IntPtr ppData);

    int GetNextString(uint ixString, out uint pNext);

    int GetNextBlob(uint ixBlob, out uint pNext);

    int GetNextGuid(uint ixGuid, out uint pNext);

    int GetNextUserString(uint ixUserString, out uint pNext);

  }

}