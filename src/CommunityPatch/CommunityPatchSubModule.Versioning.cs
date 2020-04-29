using System.Linq;
using JetBrains.Annotations;
using TaleWorlds.Library;

namespace CommunityPatch {

  public partial class CommunityPatchSubModule {

    [PublicAPI]
    public static readonly ApplicationVersionComparer VersionComparer
      = new ApplicationVersionComparer();

    [PublicAPI]
    public static ApplicationVersion GameVersion
      = GetGameVersion();

    private static ApplicationVersion GetGameVersion() {
      try {
        return ModuleInfo.GetModules()
          .Where(x => x.IsOfficial)
          .OrderByDescending(x => x.Version, VersionComparer)
          .FirstOrDefault()?.Version ?? default;
      }
      catch {
        return new ApplicationVersion(ApplicationVersionType.Invalid, 0, 0, 0, 0);
      }
    }

  }

}