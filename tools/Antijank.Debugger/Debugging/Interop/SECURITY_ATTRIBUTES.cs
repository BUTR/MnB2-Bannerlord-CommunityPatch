using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;


namespace Antijank.Interop
{
	
	[SuppressMessage("ReSharper", "IdentifierTypo")]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	[Serializable]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct SECURITY_ATTRIBUTES
	{
		public uint nLength;
		public IntPtr lpSecurityDescriptor;
		public int bInheritHandle;
	}
}
