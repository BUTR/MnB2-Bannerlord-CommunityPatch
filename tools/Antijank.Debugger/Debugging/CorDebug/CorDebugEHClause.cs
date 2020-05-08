#pragma warning disable 169
namespace Antijank.Debugging {

  public struct CorDebugEHClause {

    uint Flags;

    uint TryOffset;

    uint TryLength;

    uint HandlerOffset;

    uint HandlerLength;

    uint ClassToken;

    uint FilterOffset;

  }

}