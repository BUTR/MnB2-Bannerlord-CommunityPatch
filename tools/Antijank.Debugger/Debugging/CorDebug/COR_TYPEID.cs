using System;
using System.Runtime.InteropServices;


namespace Antijank.Debugging {

  [StructLayout(LayoutKind.Sequential)]
  
  public struct COR_TYPEID : IEquatable<COR_TYPEID> {

    public ulong token1;

    public ulong token2;

    public override int GetHashCode()
      => (int) token1 + (int) token2;

    public override bool Equals(object obj)
      => obj is COR_TYPEID a && Equals(a);

    public bool Equals(COR_TYPEID other)
      => token1 == other.token1 && token2 == other.token2;

    public bool IsMethodTable()
      => token2 == 0;

  }

}