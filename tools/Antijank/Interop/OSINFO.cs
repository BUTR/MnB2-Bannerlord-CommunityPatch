using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Interop
{
	
	[SuppressMessage("ReSharper", "IdentifierTypo")]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	[Serializable]
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct OSINFO
	{
		public uint dwOSPlatformId;
		public uint dwOSMajorVersion;
		public uint dwOSMinorVersion;
	}
}
