using System.Collections.Generic;
using JetBrains.Annotations;
using TaleWorlds.Library;

namespace CommunityPatch {

  [PublicAPI]
  public class ApplicationVersionTypeComparer : IComparer<ApplicationVersionType>, IEqualityComparer<ApplicationVersionType> {

    public int Compare(ApplicationVersionType x, ApplicationVersionType y)
      => GetHashCode(x) - GetHashCode(y);

    public bool Equals(ApplicationVersionType x, ApplicationVersionType y)
      => Compare(x, y) == 0;

    public int GetHashCode(ApplicationVersionType obj)
      => (int) obj + 1 % 5;

  }

}