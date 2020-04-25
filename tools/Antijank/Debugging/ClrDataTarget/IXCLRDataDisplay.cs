using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("A3C1704A-4559-4A67-8D28-E8F4FE3B3F62")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  [PublicAPI]
  public interface IXCLRDataDisplay {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void ErrorPrintF(ref sbyte fmt);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void NativeImageDimensions(UIntPtr @base, UIntPtr size, uint sectionAlign);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Section(ref sbyte name, UIntPtr rva, UIntPtr size);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetDumpOptions(out CLRNativeImageDumpOptions pOptions);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartDocument();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndDocument();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartCategory(ref sbyte name);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndCategory();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartElement(ref sbyte name);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndElement();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartVStructure(ref sbyte name);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartVStructureWithOffset(ref sbyte name, uint fieldOffset, uint fieldSize);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndVStructure();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartTextElement(ref sbyte name);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndTextElement();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void WriteXmlText(ref sbyte fmt);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void WriteXmlTextBlock(ref sbyte fmt);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void WriteEmptyElement(ref sbyte element);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void WriteElementPointer(ref sbyte element, UIntPtr ptr);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void WriteElementPointerAnnotated(ref sbyte element, UIntPtr ptr, ref ushort annotation);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void WriteElementAddress(ref sbyte element, UIntPtr @base, UIntPtr size);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void WriteElementAddressNamed(ref sbyte element, ref sbyte name, UIntPtr @base, UIntPtr size);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void WriteElementAddressNamedW(ref sbyte element, ref ushort name, UIntPtr @base, UIntPtr size);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void WriteElementString(ref sbyte element, ref sbyte Data);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void WriteElementStringW(ref sbyte element, ref ushort Data);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void WriteElementInt(ref sbyte element, int value);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void WriteElementUInt(ref sbyte element, uint value);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void WriteElementEnumerated(ref sbyte element, uint value, ref ushort mnemonic);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void WriteElementIntWithSuppress(ref sbyte element, int value, int suppressIfEqual);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void WriteElementFlag(ref sbyte element, int flag);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartArray(ref sbyte name, ref ushort fmt);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndArray(ref sbyte countPrefix);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartList(ref ushort fmt);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndList();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartArrayWithOffset(ref sbyte name, uint fieldOffset, uint fieldSize, ref ushort fmt);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void WriteFieldString(ref sbyte element, uint fieldOffset, uint fieldSize, ref sbyte Data);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void WriteFieldStringW(ref sbyte element, uint fieldOffset, uint fieldSize, ref ushort Data);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void WriteFieldPointer(ref sbyte element, uint fieldOffset, uint fieldSize, UIntPtr ptr);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void WriteFieldPointerWithSize(ref sbyte element, uint fieldOffset, uint fieldSize, UIntPtr ptr, UIntPtr size);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void WriteFieldInt(ref sbyte element, uint fieldOffset, uint fieldSize, int value);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void WriteFieldUInt(ref sbyte element, uint fieldOffset, uint fieldSize, uint value);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void WriteFieldEnumerated(ref sbyte element, uint fieldOffset, uint fieldSize, uint value, ref ushort mnemonic);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void WriteFieldEmpty(ref sbyte element, uint fieldOffset, uint fieldSize);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void WriteFieldFlag(ref sbyte element, uint fieldOffset, uint fieldSize, int flag);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void WriteFieldPointerAnnotated(ref sbyte element, uint fieldOffset, uint fieldSize, UIntPtr ptr,
      ref ushort annotation);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void WriteFieldAddress(ref sbyte element, uint fieldOffset, uint fieldSize, UIntPtr @base, UIntPtr size);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartStructure(ref sbyte name, UIntPtr ptr, UIntPtr size);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartStructureWithNegSpace(ref sbyte name, UIntPtr ptr, UIntPtr startPtr, UIntPtr totalSize);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartStructureWithOffset(ref sbyte name, uint fieldOffset, uint fieldSize, UIntPtr ptr, UIntPtr size);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndStructure();

  }

}