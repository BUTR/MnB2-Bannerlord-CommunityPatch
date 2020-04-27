using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  
  [ComImport, Guid("4709C9C6-81FF-11D3-9FC7-00C04F79A0A3"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  public interface IMetaDataValidate {

    int ValidatorInit(uint dwModuleType, [MarshalAs(UnmanagedType.Interface)] object pUnk);

    int ValidateMetaData();

  }

}