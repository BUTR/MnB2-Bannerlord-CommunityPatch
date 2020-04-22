﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("9FD93CCF-3280-4391-B3A9-96E1CDE77C8D")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComConversionLoss]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  [PublicAPI]
  public interface ICLRStrongName {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetHashFromAssemblyFile([MarshalAs(UnmanagedType.LPStr)] [In] string pszFilePath,
      [In] [Out] ref uint piHashAlg, out byte pbHash, [In] uint cchHash, out uint pchHash);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetHashFromAssemblyFileW([MarshalAs(UnmanagedType.LPWStr)] [In] string pwzFilePath,
      [In] [Out] ref uint piHashAlg, out byte pbHash, [In] uint cchHash, out uint pchHash);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetHashFromBlob([In] ref byte pbBlob, [In] uint cchBlob, [In] [Out] ref uint piHashAlg, out byte pbHash,
      [In] uint cchHash, out uint pchHash);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetHashFromFile([MarshalAs(UnmanagedType.LPStr)] [In] string pszFilePath, [In] [Out] ref uint piHashAlg,
      out byte pbHash, [In] uint cchHash, out uint pchHash);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetHashFromFileW([MarshalAs(UnmanagedType.LPWStr)] [In] string pwzFilePath, [In] [Out] ref uint piHashAlg,
      out byte pbHash, [In] uint cchHash, out uint pchHash);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetHashFromHandle([In] IntPtr hFile, [In] [Out] ref uint piHashAlg, out byte pbHash, [In] uint cchHash,
      out uint pchHash);

    [MethodImpl(MethodImplOptions.InternalCall)]
    uint StrongNameCompareAssemblies([MarshalAs(UnmanagedType.LPWStr)] [In] string pwzAssembly1,
      [MarshalAs(UnmanagedType.LPWStr)] [In] string pwzAssembly2);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StrongNameFreeBuffer([In] ref byte pbMemory);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StrongNameGetBlob([MarshalAs(UnmanagedType.LPWStr)] [In] string pwzFilePath, [In] [Out] ref byte pbBlob,
      [In] [Out] ref uint pcbBlob);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StrongNameGetBlobFromImage([In] ref byte pbBase, [In] uint dwLength, out byte pbBlob,
      [In] [Out] ref uint pcbBlob);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StrongNameGetPublicKey([MarshalAs(UnmanagedType.LPWStr)] [In] string pwzKeyContainer, [In] ref byte pbKeyBlob,
      [In] uint cbKeyBlob, [Out] IntPtr ppbPublicKeyBlob, out uint pcbPublicKeyBlob);

    [MethodImpl(MethodImplOptions.InternalCall)]
    uint StrongNameHashSize([In] uint ulHashAlg);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StrongNameKeyDelete([MarshalAs(UnmanagedType.LPWStr)] [In] string pwzKeyContainer);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StrongNameKeyGen([MarshalAs(UnmanagedType.LPWStr)] [In] string pwzKeyContainer, [In] uint dwFlags,
      [Out] IntPtr ppbKeyBlob, out uint pcbKeyBlob);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StrongNameKeyGenEx([MarshalAs(UnmanagedType.LPWStr)] [In] string pwzKeyContainer, [In] uint dwFlags,
      [In] uint dwKeySize, [Out] IntPtr ppbKeyBlob, out uint pcbKeyBlob);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StrongNameKeyInstall([MarshalAs(UnmanagedType.LPWStr)] [In] string pwzKeyContainer, [In] ref byte pbKeyBlob,
      [In] uint cbKeyBlob);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StrongNameSignatureGeneration([MarshalAs(UnmanagedType.LPWStr)] [In] string pwzFilePath,
      [MarshalAs(UnmanagedType.LPWStr)] [In] string pwzKeyContainer, [In] ref byte pbKeyBlob, [In] uint cbKeyBlob,
      [Out] IntPtr ppbSignatureBlob, out uint pcbSignatureBlob);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StrongNameSignatureGenerationEx([MarshalAs(UnmanagedType.LPWStr)] [In] string wszFilePath,
      [MarshalAs(UnmanagedType.LPWStr)] [In] string wszKeyContainer, [In] ref byte pbKeyBlob, [In] uint cbKeyBlob,
      [Out] IntPtr ppbSignatureBlob, out uint pcbSignatureBlob, [In] uint dwFlags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StrongNameSignatureSize([In] ref byte pbPublicKeyBlob, [In] uint cbPublicKeyBlob, [In] ref uint pcbSize);

    [MethodImpl(MethodImplOptions.InternalCall)]
    uint StrongNameSignatureVerification([MarshalAs(UnmanagedType.LPWStr)] [In] string pwzFilePath,
      [In] uint dwInFlags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    sbyte StrongNameSignatureVerificationEx([MarshalAs(UnmanagedType.LPWStr)] [In] string pwzFilePath,
      [In] sbyte fForceVerification);

    [MethodImpl(MethodImplOptions.InternalCall)]
    uint StrongNameSignatureVerificationFromImage([In] ref byte pbBase, [In] uint dwLength, [In] uint dwInFlags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StrongNameTokenFromAssembly([MarshalAs(UnmanagedType.LPWStr)] [In] string pwzFilePath,
      [Out] IntPtr ppbStrongNameToken, out uint pcbStrongNameToken);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StrongNameTokenFromAssemblyEx([MarshalAs(UnmanagedType.LPWStr)] [In] string pwzFilePath,
      [Out] IntPtr ppbStrongNameToken, out uint pcbStrongNameToken, [Out] IntPtr ppbPublicKeyBlob,
      out uint pcbPublicKeyBlob);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StrongNameTokenFromPublicKey([In] ref byte pbPublicKeyBlob, [In] uint cbPublicKeyBlob,
      [Out] IntPtr ppbStrongNameToken, out uint pcbStrongNameToken);

  }

}