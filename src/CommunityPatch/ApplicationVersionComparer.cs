using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TaleWorlds.Library;

namespace CommunityPatch {

  public class ApplicationVersionComparer : IComparer<ApplicationVersion>, IEqualityComparer<ApplicationVersion> {

    private readonly ApplicationVersionTypeComparer _typeComparer = new ApplicationVersionTypeComparer();

    public int Compare(ApplicationVersion x, ApplicationVersion y) {
      var typeCmpResult = _typeComparer.Compare(x.ApplicationVersionType, y.ApplicationVersionType);
      if (typeCmpResult != 0)
        return typeCmpResult;

      var majorCmpResult = x.Major.CompareTo(y.Major);
      if (majorCmpResult != 0)
        return majorCmpResult;

      var minorCmpResult = x.Minor.CompareTo(y.Minor);
      return minorCmpResult != 0 ? minorCmpResult : x.Revision.CompareTo(y.Revision);
    }

    public bool Equals(ApplicationVersion x, ApplicationVersion y)
      => Compare(x, y) == 0;

    public bool GreaterThan(ApplicationVersion x, ApplicationVersion y) => Compare(x, y) > 0;

    public int GetHashCode(ApplicationVersion obj)
    => ((int) _typeComparer.GetHashCode(obj.ApplicationVersionType) << 28) | (obj.Major << 23) | (obj.Minor << 12) | obj.Revision;

  }

}