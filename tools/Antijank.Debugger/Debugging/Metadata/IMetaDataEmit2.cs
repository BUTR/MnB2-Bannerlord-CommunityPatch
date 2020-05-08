using System;
using System.Runtime.InteropServices;


namespace Antijank.Debugging {

  
  [Guid("F5DD9950-F693-42e6-830E-7B833E8146A9"), ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  public interface IMetaDataEmit2 {

    void DefineMethodSpec(uint tkParent, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
      byte[] pvSigBlob, uint cbSigBlob, out uint pmi);

    void GetDeltaSaveSize(CorSaveSize fSave, out uint pdwSaveSize);

    void SaveDelta([MarshalAs(UnmanagedType.LPWStr)] string szFile, uint dwSaveFlags);

    void SaveDeltaToStream([MarshalAs(UnmanagedType.Interface)] object pIStream, uint dwSaveFlags);

    void SaveDeltaToMemory(IntPtr pbData, uint cbData);

    void DefineGenericParam(
      uint tk,
      uint ulParamSeq,
      uint dwParamFlags,
      [MarshalAs(UnmanagedType.LPWStr)] string szname,
      uint tkKind,
      [MarshalAs(UnmanagedType.LPArray)] uint[] rtkConstraints,
      out uint pgp
    );

    void SetGenericParamProps(
      uint gp,
      uint dwParamFlags,
      [MarshalAs(UnmanagedType.LPWStr)] string szName,
      uint tkKind,
      [MarshalAs(UnmanagedType.LPArray)] uint[] rtkConstraints
    );

    void ResetENCLog();

  }

}