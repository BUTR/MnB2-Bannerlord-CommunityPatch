using System.Runtime.InteropServices;

namespace Antijank.Interop {

  [Guid("D57C7288-D4AD-4768-BE02-9D969532D960")]
  [CoClass(typeof(FileOpenDialogClass))]
  [ComImport]
  public interface FileOpenDialog : IFileOpenDialog {

  }

}