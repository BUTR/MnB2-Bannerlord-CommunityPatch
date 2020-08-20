#if !AFTER_E1_4_3

using TaleWorlds.Library;

namespace CommunityPatch.Patches.Perks.Endurance.Riding {

  [PatchObsolete(ApplicationVersionType.EarlyAccess,1,4, 3)]
  public sealed class ConroiPatch : PartySizeLimitSubPatch<ConroiPatch> {

    public ConroiPatch() : base("k15fVuc4") { }

  }

}

#endif