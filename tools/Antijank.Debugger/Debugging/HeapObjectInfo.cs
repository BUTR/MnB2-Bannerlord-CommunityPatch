#nullable enable
using System;
using System.ComponentModel;

namespace Antijank.Debugging {

  public readonly struct HeapObjectInfo : IHeapObjectInfo {

    public readonly object? Value;

    object? IHeapObjectInfo.Value
      => Value;

    public readonly CorElementType ElementType;

    CorElementType IHeapObjectInfo.ElementType
      => ElementType;

    public readonly ulong Size;

    ulong IHeapObjectInfo.Size
      => Size;

    public readonly uint HeapId;

    uint IHeapObjectInfo.HeapId
      => HeapId;

    public readonly ulong HeapBaseAddress;

    ulong IHeapObjectInfo.HeapBaseAddress
      => HeapBaseAddress;

    public readonly CorDebugGenerationTypes HeapType;

    CorDebugGenerationTypes IHeapObjectInfo.HeapType
      => HeapType;

    public HeapObjectInfo(
      object objRef,
      CorElementType elementType,
      ulong objSize,
      uint heapId,
      ulong heapBaseAddress,
      CorDebugGenerationTypes heapRegionType
    ) {
      Value = objRef;
      Size = objSize;
      HeapId = heapId;
      HeapBaseAddress = heapBaseAddress;
      HeapType = heapRegionType;
      ElementType = elementType;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj)
      => throw new NotSupportedException();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode()
      => throw new NotSupportedException();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string ToString()
      => throw new NotSupportedException();

  }

}