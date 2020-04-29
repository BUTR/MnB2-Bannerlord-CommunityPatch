using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Antijank.Interop
{
	[Guid("00000100-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[SuppressUnmanagedCodeSecurity]
	[ComImport]
	public interface IEnumUnknown
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: Description("pcEltFetched")]
		uint Next([In] uint celt, [MarshalAs(UnmanagedType.IUnknown)] out object rgElt);
		[MethodImpl(MethodImplOptions.InternalCall)]
		void Skip([In] uint celt);
		[MethodImpl(MethodImplOptions.InternalCall)]
		void Reset();
		[MethodImpl(MethodImplOptions.InternalCall)]
		void Clone([MarshalAs(UnmanagedType.Interface)] out IEnumUnknown ppEnum);
	}
}
