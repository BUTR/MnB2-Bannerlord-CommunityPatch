using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  
  [Guid("06A3EA8B-0225-11d1-BF72-00C04FC31E12"), ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  public interface IMapToken {

    int Map(uint tkImp, uint tkEmit);

  }

}