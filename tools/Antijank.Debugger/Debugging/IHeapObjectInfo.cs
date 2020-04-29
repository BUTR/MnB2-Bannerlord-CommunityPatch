#nullable enable

namespace Antijank.Debugging {

  public interface IHeapObjectInfo {

    object? Value { get; }

    CorElementType ElementType { get; }

    ulong Size { get; }

    uint HeapId { get; }

    ulong HeapBaseAddress { get; }

    CorDebugGenerationTypes HeapType { get; }

  }

}