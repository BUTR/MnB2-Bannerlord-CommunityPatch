using System.Runtime.InteropServices;

namespace Antijank.Interop
{
  [Guid("84BCCD23-5FDE-4CDB-AEA4-AF64B83D78AB")]
  [CoClass(typeof (FileSaveDialogClass))]
  [ComImport]
  public interface FileSaveDialog : IFileSaveDialog
  {
  }
}
