using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [PublicAPI]
  [Guid("F5DD9950-F693-42e6-830E-7B833E8146A9"), ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  public interface IMetaDataEmit2 {

    int DefineMethodSpec(uint tkParent, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
      byte[] pvSigBlob, uint cbSigBlob, uint pmi);

    int GetDeltaSaveSize(CorSaveSize fSave, uint pdwSaveSize);

    int SaveDelta([MarshalAs(UnmanagedType.LPWStr)] string szFile, uint dwSaveFlags);

    int SaveDeltaToStream([MarshalAs(UnmanagedType.Interface)] object pIStream, uint dwSaveFlags);

    int SaveDeltaToMemory(IntPtr pbData, uint cbData);

    int DefineGenericParam(
      uint tk,
      uint ulParamSeq,
      uint dwParamFlags,
      [MarshalAs(UnmanagedType.LPWStr)] string szname,
      uint tkKind,
      [MarshalAs(UnmanagedType.LPArray)] uint[] rtkConstraints,
      out uint pgp
    );

    int SetGenericParamProps(
      uint gp,
      uint dwParamFlags,
      [MarshalAs(UnmanagedType.LPWStr)] string szName,
      uint tkKind,
      [MarshalAs(UnmanagedType.LPArray)] uint[] rtkConstraints
    );

    int ResetENCLog();

  }

}