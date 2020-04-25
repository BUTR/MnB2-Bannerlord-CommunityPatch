using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Antijank.Interop {

  [PublicAPI]
  [SuppressMessage("ReSharper", "IdentifierTypo")]
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public struct iovec
  {
    public IntPtr iov_base; /* Starting address */
    public UIntPtr iov_len; /* Number of bytes to transfer */
  }

}