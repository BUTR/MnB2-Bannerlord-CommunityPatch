using System;
using TaleWorlds.Library;

namespace Antijank {

  public struct WeightedModuleInfo : IComparable<WeightedModuleInfo> {

    public int CompareTo(WeightedModuleInfo other)
      => Weight.CompareTo(other.Weight);

    public static bool operator <(WeightedModuleInfo left, WeightedModuleInfo right)
      => left.CompareTo(right) < 0;

    public static bool operator >(WeightedModuleInfo left, WeightedModuleInfo right)
      => left.CompareTo(right) > 0;

    public static bool operator <=(WeightedModuleInfo left, WeightedModuleInfo right)
      => left.CompareTo(right) <= 0;

    public static bool operator >=(WeightedModuleInfo left, WeightedModuleInfo right)
      => left.CompareTo(right) >= 0;

    public bool Equals(WeightedModuleInfo other)
      => Equals(Module, other.Module);

    public override bool Equals(object obj)
      => obj is WeightedModuleInfo other && Equals(other);

    public override int GetHashCode()
      => (Module != null ? Module.GetHashCode() : 0);

    public static bool operator ==(WeightedModuleInfo left, WeightedModuleInfo right)
      => left.Equals(right);

    public static bool operator !=(WeightedModuleInfo left, WeightedModuleInfo right)
      => !left.Equals(right);

    public static readonly WeightedModuleInfo Invalid = default;

    public readonly ModuleInfo Module;

    public int Weight;

    public WeightedModuleInfo(ModuleInfo mod, int weight) {
      Module = mod;
      Weight = weight;
    }
    
  }

}