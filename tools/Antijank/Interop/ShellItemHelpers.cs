using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;

namespace Antijank.Interop {

  public class ShellItemHelpers {

    [DllImport("shell32")]
    [SecurityCritical, SuppressUnmanagedCodeSecurity]
    [return: MarshalAs(UnmanagedType.Error)]
    private static extern uint SHCreateItemFromParsingName(
      [MarshalAs(UnmanagedType.LPWStr)] string pszPath,
      IBindCtx pbc,
      [In] ref Guid riid,
      [MarshalAs(UnmanagedType.Interface)] out IShellItem ppv
    );

    [SecurityCritical]
    public static IShellItem Parse(string path) {
      if (string.IsNullOrEmpty(path))
        return null;

      var iid = typeof(IShellItem).GUID;
      var hr = SHCreateItemFromParsingName(path, null, ref iid, out var createdItem);

      // ERROR_FILE_NOT_FOUND : 0x80070002
      // ERROR_PATH_NOT_FOUND : 0x80070003
      if (hr == 0)
        return createdItem;

      throw Marshal.GetExceptionForHR((int) hr);
    }

  }

}