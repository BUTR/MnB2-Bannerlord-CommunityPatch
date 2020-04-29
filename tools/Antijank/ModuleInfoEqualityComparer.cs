using System.Collections.Generic;
using TaleWorlds.Library;

namespace Antijank {

  public class ModuleInfoEqualityComparer : IEqualityComparer<ModuleInfo> {

    public bool Equals(ModuleInfo x, ModuleInfo y)
      => x?.Id.Equals(y?.Id) ?? y == null;

    public int GetHashCode(ModuleInfo obj)
      => obj.GetHashCode();

  }

}