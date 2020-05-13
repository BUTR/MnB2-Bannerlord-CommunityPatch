using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Antijank.Interop
{
	
	[SuppressMessage("ReSharper", "IdentifierTypo")]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	[Serializable]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct STATSTG
	{
		[MarshalAs(UnmanagedType.LPWStr)]
		public string pwcsName;
		public uint type;
		public ulong cbSize;
		public FILETIME mtime;
		public FILETIME ctime;
		public FILETIME atime;
		public uint grfMode;
		public uint grfLocksSupported;
		public Guid clsid;
		public uint grfStateBits;
		public uint reserved;
	}
}
