using System.Runtime.InteropServices;

namespace Antijank.Interop {

  [Guid("7E9FB0D3-919F-4307-AB2E-9B1860310C93")]
  [CoClass(typeof(ShellItemClass))]
  [ComImport]
  public interface ShellItem : IShellItem2 {

  }

}