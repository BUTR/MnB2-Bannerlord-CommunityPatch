using TaleWorlds.Library;

namespace Antijank {

  public struct WeightedModuleInfo {

    public ModuleInfo Module;

    public int Weight;

    public WeightedModuleInfo(ModuleInfo mod, int weight) {
      Module = mod;
      Weight = weight;
    }

  }

}