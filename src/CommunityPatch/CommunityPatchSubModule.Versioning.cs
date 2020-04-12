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
      = ModuleInfo.GetModules()
        .Where(x => x.IsOfficial)
        .OrderByDescending(x => x.Version, VersionComparer)
        .FirstOrDefault()?.Version ?? default;

  }

}