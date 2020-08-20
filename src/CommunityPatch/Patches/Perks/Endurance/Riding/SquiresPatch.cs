#if !AFTER_E1_4_3

using TaleWorlds.Library;

namespace CommunityPatch.Patches.Perks.Endurance.Riding {

  [PatchObsolete(ApplicationVersionType.EarlyAccess,1,4, 3)]
  public sealed class SquiresPatch : PartySizeLimitSubPatch<SquiresPatch> {

    public SquiresPatch() : base("qaAKXRSV") { }

  }

}

#endif