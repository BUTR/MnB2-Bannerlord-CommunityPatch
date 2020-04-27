using TaleWorlds.Library;

namespace BannerlordModuleManagement {

  internal readonly struct WeightedModuleInfo {

    public readonly ModuleInfo Module;

    public readonly int Weight;

    public WeightedModuleInfo(ModuleInfo mod, int weight) {
      Module = mod;
      Weight = weight;
    }

  }

}