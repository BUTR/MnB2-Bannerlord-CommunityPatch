using JetBrains.Annotations;

namespace Antijank.Debugging {

  public interface IHeapObjectInfo {

    [CanBeNull]
    object Value { get; }

    CorElementType ElementType { get; }

    ulong Size { get; }

    uint HeapId { get; }

    ulong HeapBaseAddress { get; }

    CorDebugGenerationTypes HeapType { get; }

  }

}