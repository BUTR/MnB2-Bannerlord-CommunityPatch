using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Interop
{
	[PublicAPI]
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
