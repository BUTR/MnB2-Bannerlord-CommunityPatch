using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.Library;

namespace CommunityPatch {

  public class ApplicationVersionComparer : IComparer<ApplicationVersion>, IEqualityComparer<ApplicationVersion> {

    private readonly ApplicationVersionTypeComparer _typeComparer = new ApplicationVersionTypeComparer();

    [CanBeNull] private static readonly MethodInfo ChangeSetGetter;

    static ApplicationVersionComparer() {
      try {
        // this must not be explicitly referenced for backwards compat; symbol did not exist
        ChangeSetGetter = AccessTools.PropertyGetter(typeof(ApplicationVersion), "ChangeSet");
      }
      catch {
        // nope
      }
    }

    public int Compare(ApplicationVersion x, ApplicationVersion y) {
      var typeCmpResult = _typeComparer.Compare(x.ApplicationVersionType, y.ApplicationVersionType);
      if (typeCmpResult != 0)
        return typeCmpResult;

      var majorCmpResult = x.Major.CompareTo(y.Major);
      if (majorCmpResult != 0)
        return majorCmpResult;

      var minorCmpResult = x.Minor.CompareTo(y.Minor);
      if (minorCmpResult != 0)
        return minorCmpResult;

      var revCmpResult = x.Revision.CompareTo(y.Revision);
      if (revCmpResult != 0)
        return revCmpResult;

      if (ChangeSetGetter == null)
        return revCmpResult;

      var xChgSet = (int) ChangeSetGetter.Invoke(x, null);
      var yChgSet = (int) ChangeSetGetter.Invoke(y, null);

      return xChgSet.CompareTo(yChgSet);
    }

    public bool Equals(ApplicationVersion x, ApplicationVersion y)
      => Compare(x, y) == 0;

    public int GetHashCode(ApplicationVersion obj)
      => (_typeComparer.GetHashCode(obj.ApplicationVersionType) << 29)
        | (obj.Major << 22)
        | (obj.Minor << 15)
        | (obj.Revision << 8)
        | (ChangeSetGetter != null
          ? (int) ChangeSetGetter.Invoke(obj, null)
          : 0);

    [PublicAPI]
    public bool GreaterThan(ApplicationVersion x, ApplicationVersion y)
      => Compare(x, y) > 0;

    [PublicAPI]
    public bool LessThan(ApplicationVersion x, ApplicationVersion y)
      => Compare(x, y) < 0;

    [PublicAPI]
    public bool GreaterThanOrEqualTo(ApplicationVersion x, ApplicationVersion y)
      => Compare(x, y) >= 0;

    [PublicAPI]
    public bool LessThanOrEqualTo(ApplicationVersion x, ApplicationVersion y)
      => Compare(x, y) <= 0;

  }

}